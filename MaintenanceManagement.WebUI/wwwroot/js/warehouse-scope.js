/* Ambar hareketleri: stok girişi, kaynak malzeme ve hedef ambar seçimleri. */
(function () {
    'use strict';

    function initialize() {
        const stockW = document.getElementById('stockWarehouse');
        const stockM = document.getElementById('stockMaterial');
        const source = document.getElementById('transferSource');
        const material = document.getElementById('transferMaterial');
        const target = document.getElementById('transferTarget');

        function resetSelect(select, label) {
            if (!select) return;
            select.replaceChildren(new Option(label, ''));
            select.value = '';
            select.disabled = true;
        }

        function showError(id, message) {
            const el = document.getElementById(id);
            if (el) el.textContent = message || '';
        }

        async function loadJson(url) {
            const response = await fetch(url, {
                credentials: 'same-origin',
                headers: { 'Accept': 'application/json' }
            });
            if (!response.ok) {
                let message = 'Kayıtlar yüklenemedi (HTTP ' + response.status + ').';
                try {
                    const payload = await response.json();
                    message = payload.message || payload.title || message;
                } catch (_) { /* Sunucu JSON döndürmeyebilir. */ }
                throw new Error(message);
            }
            const rows = await response.json();
            if (!Array.isArray(rows)) {
                throw new Error('Sunucu beklenen malzeme/ambar listesini döndürmedi.');
            }
            return rows;
        }

        function fillSelect(select, rows, selected) {
            for (const row of rows) {
                const id = row.id ?? row.Id;
                const text = row.text ?? row.Text ?? '';
                const path = row.organizationPath ?? row.OrganizationPath ?? '';
                if (id == null) continue;
                select.add(new Option(path ? path + ' / ' + text : text, String(id)));
            }
            select.disabled = select.options.length <= 1;
            if (selected != null && String(selected) !== '') {
                const exists = Array.from(select.options).some(option => option.value === String(selected));
                if (exists) select.value = String(selected);
            }
        }

        let stockRequest = 0;
        async function loadStock() {
            const request = ++stockRequest;
            resetSelect(stockM, 'Malzemeler yükleniyor...');
            showError('stockScopeError', '');
            if (!stockW.value) {
                resetSelect(stockM, 'Önce ambar seçiniz...');
                return;
            }
            try {
                const materialUrl = stockW.dataset.materialUrl;
                if (!materialUrl) throw new Error('Malzeme listeleme adresi tanımlanmamış.');
                const url = new URL(materialUrl, window.location.href);
                url.searchParams.set('warehouseId', stockW.value);
                url.searchParams.set('onlyStocked', stockW.dataset.onlyStocked === 'true' ? 'true' : 'false');
                const rows = await loadJson(url);
                if (request !== stockRequest) return;
                resetSelect(stockM, rows.length ? 'Malzeme seçiniz...' : 'Uygun malzeme bulunamadı');
                fillSelect(stockM, rows, stockM.dataset.selected || stockW.dataset.currentMaterial);
                stockM.dataset.selected = '';
                stockW.dataset.currentMaterial = '';
                if (rows.length === 0) {
                    showError('stockScopeError', 'Bu ambar için aktif ve kategori kapsamına uygun malzeme bulunamadı.');
                }
            } catch (error) {
                if (request !== stockRequest) return;
                resetSelect(stockM, 'Malzemeler yüklenemedi');
                showError('stockScopeError', error.message || 'Malzemeler yüklenemedi.');
                console.error('Stok malzeme yükleme hatası:', error);
            }
        }

        let sourceRequest = 0;
        let targetRequest = 0;
        async function loadSourceMaterials() {
            const request = ++sourceRequest;
            ++targetRequest;
            resetSelect(material, 'Malzemeler yükleniyor...');
            resetSelect(target, 'Önce malzeme seçiniz...');
            showError('transferScopeError', '');
            if (!source.value) {
                resetSelect(material, 'Önce kaynak ambar seçiniz...');
                return;
            }
            try {
                if (!source.dataset.materialUrl) throw new Error('Malzeme listeleme adresi tanımlanmamış.');
                const url = new URL(source.dataset.materialUrl, window.location.href);
                url.searchParams.set('warehouseId', source.value);
                url.searchParams.set('onlyStocked', 'true');
                const rows = await loadJson(url);
                if (request !== sourceRequest) return;
                resetSelect(material, rows.length ? 'Malzeme seçiniz...' : 'Stoklu malzeme bulunamadı');
                fillSelect(material, rows, source.dataset.selectedMaterial);
                source.dataset.selectedMaterial = '';
                if (rows.length === 0) {
                    showError('transferScopeError', 'Kaynak ambarda kategori kapsamına uygun ve stoklu malzeme bulunamadı.');
                }
                if (material.value) await loadTargets();
            } catch (error) {
                if (request !== sourceRequest) return;
                resetSelect(material, 'Malzemeler yüklenemedi');
                showError('transferScopeError', error.message || 'Malzemeler yüklenemedi.');
                console.error('Transfer malzeme yükleme hatası:', error);
            }
        }

        async function loadTargets() {
            const request = ++targetRequest;
            resetSelect(target, 'Hedef ambarlar yükleniyor...');
            showError('transferScopeError', '');
            if (!source.value || !material.value) {
                resetSelect(target, 'Önce malzeme seçiniz...');
                return;
            }
            try {
                if (!target.dataset.targetUrl) throw new Error('Hedef ambar listeleme adresi tanımlanmamış.');
                const url = new URL(target.dataset.targetUrl, window.location.href);
                url.searchParams.set('sourceWarehouseId', source.value);
                url.searchParams.set('materialId', material.value);
                const rows = await loadJson(url);
                if (request !== targetRequest) return;
                resetSelect(target, rows.length ? 'Hedef ambar seçiniz...' : 'Aktif hedef ambar bulunamadı');
                fillSelect(target, rows, target.dataset.selectedTarget);
                target.dataset.selectedTarget = '';
                if (rows.length === 0) {
                    showError('transferScopeError', 'Kaynak ambar dışındaki aktif hedef ambarlar bulunamadı.');
                }
            } catch (error) {
                if (request !== targetRequest) return;
                resetSelect(target, 'Hedef ambarlar yüklenemedi');
                showError('transferScopeError', error.message || 'Hedef ambarlar yüklenemedi.');
                console.error('Hedef ambar yükleme hatası:', error);
            }
        }

        if (stockW && stockM) {
            stockW.addEventListener('change', loadStock);
            loadStock();
        }
        if (source && material && target) {
            source.addEventListener('change', loadSourceMaterials);
            material.addEventListener('change', loadTargets);
            loadSourceMaterials();
        }
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initialize, { once: true });
    } else {
        initialize();
    }
})();

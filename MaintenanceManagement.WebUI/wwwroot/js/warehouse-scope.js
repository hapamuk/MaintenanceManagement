(function () {
    'use strict';
    const stockW = document.getElementById('stockWarehouse');
    const stockM = document.getElementById('stockMaterial');
    const source = document.getElementById('transferSource');
    const material = document.getElementById('transferMaterial');
    const target = document.getElementById('transferTarget');

    function clearSelect(select, label) {
        if (!select) return;
        select.replaceChildren(new Option(label, ''));
        select.value = '';
    }
    function showError(id, message) {
        const el = document.getElementById(id);
        if (el) el.textContent = message || '';
    }
    async function loadJson(url) {
        const r = await fetch(url, { credentials: 'same-origin', headers: {'Accept':'application/json'} });
        if (!r.ok) {
            let message = 'Uygun kayıtlar yüklenemedi.';
            try { const error = await r.json(); message = error.message || message; } catch (_) {}
            throw new Error(message);
        }
        return await r.json();
    }
    function addOptions(select, rows, selected) {
        rows.forEach(x => {
            const text = x.text || x.Text || '';
            const id = String(x.id ?? x.Id);
            const label = x.organizationPath || x.OrganizationPath || '';
            // Hedef seçiminde organizasyon yolu görünür; numerik UnitId gösterilmez.
            const value = label ? label + '  /  ' + text : text;
            select.add(new Option(value, id));
        });
        if (selected) select.value = String(selected);
    }
    async function loadStock() {
        clearSelect(stockM, 'Malzeme seçiniz...');
        showError('stockScopeError', '');
        if (!stockW.value) return;
        try {
            const url = new URL(stockW.dataset.materialUrl, window.location.href);
            url.searchParams.set('warehouseId', stockW.value);
            url.searchParams.set('onlyStocked', stockW.dataset.onlyStocked === 'true' ? 'true' : 'false');
            const rows = await loadJson(url);
            addOptions(stockM, rows, stockM.dataset.selected || stockW.dataset.currentMaterial);
            stockM.dataset.selected = '';
            stockW.dataset.currentMaterial = '';
            if (!rows.length) showError('stockScopeError', 'Bu ambar için uygun malzeme bulunamadı.');
        } catch (e) { showError('stockScopeError', e.message); }
    }
    async function loadSourceMaterials() {
        clearSelect(material, 'Malzeme seçiniz...');
        clearSelect(target, 'Önce malzeme seçiniz...');
        showError('transferScopeError', '');
        if (!source.value) return;
        try {
            const url = new URL(source.dataset.materialUrl, window.location.href);
            url.searchParams.set('warehouseId', source.value);
            url.searchParams.set('onlyStocked', 'true');
            const rows = await loadJson(url);
            addOptions(material, rows, source.dataset.selectedMaterial);
            source.dataset.selectedMaterial = '';
            if (!rows.length) showError('transferScopeError', 'Kaynak ambarda uygun ve stoklu malzeme yok.');
            if (material.value) await loadTargets();
        } catch (e) { showError('transferScopeError', e.message); }
    }
    async function loadTargets() {
        clearSelect(target, 'Hedef ambar seçiniz...');
        showError('transferScopeError', '');
        if (!source.value || !material.value) return;
        try {
            const url = new URL(target.dataset.targetUrl, window.location.href);
            url.searchParams.set('sourceWarehouseId', source.value);
            url.searchParams.set('materialId', material.value);
            const rows = await loadJson(url);
            addOptions(target, rows, target.dataset.selectedTarget);
            target.dataset.selectedTarget = '';
            if (!rows.length) showError('transferScopeError', 'Bu malzemenin kategorisine uygun başka aktif ambar yok.');
        } catch (e) { showError('transferScopeError', e.message); }
    }
    if (stockW && stockM) { stockW.addEventListener('change', loadStock); loadStock(); }
    if (source && material && target) {
        source.addEventListener('change', loadSourceMaterials);
        material.addEventListener('change', loadTargets);
        loadSourceMaterials();
    }
})();

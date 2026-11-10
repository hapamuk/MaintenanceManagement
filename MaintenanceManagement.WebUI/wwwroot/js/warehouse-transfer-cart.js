(() => {
  'use strict';
  const source = document.getElementById('transferSource');
  const target = document.getElementById('transferTarget');
  const catalog = document.getElementById('transferCatalog');
  const basket = document.getElementById('transferBasket');
  const hidden = document.getElementById('transferHiddenItems');
  const search = document.getElementById('transferSearch');
  const error = document.getElementById('transferScopeError');
  const submit = document.getElementById('transferSubmit');
  const form = document.getElementById('transferCartForm');
  if (!source || !target || !catalog || !basket || !hidden || !form || !submit || !search || !error) return;

  let available = [];
  const cart = new Map();
  const draftQuantities = new Map();
  let requestNumber = 0;
  let controller = null;
  const count = document.getElementById('transferAvailableCount');
  const cartCount = document.getElementById('transferCartCount');
  const coalesce = (x, fallback) => x == null ? fallback : x;
  const decimalNumber = value => Number(String(value).trim().replace(',', '.'));
  const quantityText = value => Number(value).toLocaleString('tr-TR', { maximumFractionDigits: 4 });

  function empty(el, icon, message) {
    el.replaceChildren();
    const box = document.createElement('div'); box.className = 'transfer-empty';
    const i = document.createElement('i'); i.className = `fas fa-${icon}`;
    const p = document.createElement('p'); p.textContent = message;
    box.append(i, p); el.append(box);
  }
  function setError(message = '') { error.textContent = message; }
  function refreshTargets() {
    [...target.options].forEach(o => { if (o.value) o.disabled = o.value === source.value; });
    if (target.value === source.value) target.value = '';
  }
  function clearCart() { cart.clear(); draftQuantities.clear(); renderCart(); }

  async function loadAvailable() {
    const requestId = ++requestNumber;
    if (controller) controller.abort();
    controller = new AbortController();
    clearCart(); available = []; count.textContent = '0 malzeme'; setError();
    refreshTargets();
    if (!source.value || !target.value) {
      empty(catalog, 'warehouse', 'Hedef ambarı seçiniz.'); return;
    }
    empty(catalog, 'spinner fa-spin', 'Uygun malzemeler yükleniyor...');
    const url = new URL(target.dataset.materialsUrl, location.href);
    url.searchParams.set('sourceWarehouseId', source.value);
    url.searchParams.set('targetWarehouseId', target.value);
    try {
      const response = await fetch(url, {
        credentials: 'same-origin', headers: { Accept: 'application/json' }, signal: controller.signal
      });
      const body = await response.json();
      if (!response.ok) throw new Error(body.message || 'Malzemeler yüklenemedi.');
      if (requestId !== requestNumber) return;
      if (!Array.isArray(body)) throw new Error('Sunucu geçersiz veri döndürdü.');
      available = body.map(x => ({
        id: Number(coalesce(x.id, x.Id)),
        text: String(coalesce(x.text, x.Text) || ''),
        available: decimalNumber(coalesce(x.availableQuantity, x.AvailableQuantity)),
        unit: String(coalesce(x.unit, x.Unit) || '')
      })).filter(x => Number.isInteger(x.id) && x.id > 0 && Number.isFinite(x.available) && x.available > 0);
      renderCatalog();
    } catch (ex) {
      if (ex.name === 'AbortError' || requestId !== requestNumber) return;
      empty(catalog, 'exclamation-circle', 'Liste yüklenemedi.'); setError(ex.message);
    }
  }

  function renderCatalog() {
    catalog.replaceChildren();
    const needle = search.value.trim().toLocaleLowerCase('tr-TR');
    const items = available.filter(x => x.text.toLocaleLowerCase('tr-TR').includes(needle));
    count.textContent = `${available.length} malzeme`;
    if (!items.length) {
      empty(catalog, 'box-open', available.length ? 'Aramanıza uygun malzeme yok.' : 'Bu hedef ambara aktarılabilecek stoklu malzeme bulunamadı.');
      return;
    }

    for (const item of items) {
      const inCart = cart.has(item.id);
      const el = document.createElement('div'); el.className = 'transfer-material';
      const main = document.createElement('div'); main.className = 'transfer-material-main';
      const name = document.createElement('div'); name.className = 'transfer-material-title'; name.textContent = item.text;
      const stock = document.createElement('div'); stock.className = 'transfer-stock';
      stock.textContent = `Kaynak stok: ${quantityText(item.available)} ${item.unit}`;
      main.append(name, stock);

      const controls = document.createElement('div'); controls.className = 'transfer-catalog-controls';
      const label = document.createElement('label');
      label.className = 'transfer-catalog-qty-label'; label.textContent = 'Transfer miktarı';
      const qty = document.createElement('input');
      qty.type = 'number'; qty.inputMode = 'decimal'; qty.min = '0';
      // step=any allows manual fractional amounts; native spinners step by 1.
      qty.max = String(Math.min(999999999, item.available)); qty.step = 'any';
      qty.className = 'form-control form-control-sm transfer-catalog-qty';
      qty.setAttribute('aria-label', `${item.text} transfer miktarı`);
      qty.value = String(inCart ? cart.get(item.id).quantity : (draftQuantities.get(item.id) ?? 1));
      qty.disabled = inCart;
      const btn = document.createElement('button'); btn.type = 'button';
      btn.className = 'btn btn-sm btn-outline-primary';
      btn.disabled = inCart;
      btn.textContent = inCart ? 'Sepette' : '+ Sepete Ekle';
      const validation = document.createElement('small'); validation.className = 'transfer-limit';
      validation.setAttribute('role', 'status');

      function checkQuantity() {
        const raw = qty.value;
        const val = decimalNumber(raw);
        const valid = raw !== '' && Number.isFinite(val) && val >= 0.0001 && val <= item.available && val <= 999999999;
        if (!inCart) {
          btn.disabled = !valid;
          qty.classList.toggle('is-invalid', !valid);
          validation.textContent = valid ? '' : `Miktar 0'dan büyük ve stoktan (${quantityText(item.available)}) fazla olmamalı.`;
        }
        return valid;
      }
      qty.addEventListener('input', () => { draftQuantities.set(item.id, qty.value); checkQuantity(); });
      // Keyboard arrows always add/subtract 1 rather than 0.0001.
      qty.addEventListener('keydown', event => {
        if (event.key !== 'ArrowUp' && event.key !== 'ArrowDown') return;
        event.preventDefault();
        const current = decimalNumber(qty.value);
        const base = Number.isFinite(current) ? current : 0;
        const next = Math.round((base + (event.key === 'ArrowUp' ? 1 : -1)) * 10000) / 10000;
        qty.value = String(Math.max(0, Math.min(item.available, 999999999, next)));
        draftQuantities.set(item.id, qty.value);
        checkQuantity();
      });
      btn.addEventListener('click', () => {
        if (!checkQuantity()) return;
        const value = decimalNumber(qty.value);
        cart.set(item.id, { item, quantity: value });
        renderCart(); renderCatalog();
      });
      label.append(qty);
      controls.append(label, btn, validation);
      el.append(main, controls); catalog.append(el);
      if (!inCart) checkQuantity();
    }
  }

  function renderCart() {
    basket.replaceChildren(); hidden.replaceChildren();
    cartCount.textContent = `${cart.size} kalem`;
    if (!cart.size) {
      empty(basket, 'shopping-basket', 'Solda miktarı belirleyip Sepete Ekle butonuna basınız.');
      validate(); return;
    }
    [...cart.values()].forEach(({ item, quantity }, idx) => {
      const box = document.createElement('div'); box.className = 'transfer-basket-item';
      const head = document.createElement('div'); head.className = 'transfer-basket-head';
      const name = document.createElement('strong'); name.className = 'transfer-material-title'; name.textContent = item.text;
      const remove = document.createElement('button'); remove.type = 'button';
      remove.className = 'btn btn-sm btn-outline-danger'; remove.textContent = 'Kaldır';
      remove.addEventListener('click', () => {
        cart.delete(item.id);
        draftQuantities.set(item.id, String(quantity));
        renderCart(); renderCatalog();
      });
      head.append(name, remove);
      const info = document.createElement('div'); info.className = 'transfer-cart-quantity';
      info.textContent = `Aktarılacak miktar: ${quantityText(quantity)} ${item.unit}`;
      const note = document.createElement('small'); note.className = 'transfer-stock';
      note.textContent = 'Miktarı değiştirmek için sepetten kaldırıp soldan yeniden ekleyin.';
      const hiddenQty = document.createElement('input'); hiddenQty.type = 'hidden';
      hiddenQty.name = `Items[${idx}].Quantity`; hiddenQty.value = String(quantity);
      const hiddenId = document.createElement('input'); hiddenId.type = 'hidden';
      hiddenId.name = `Items[${idx}].MaterialId`; hiddenId.value = String(item.id);
      box.append(head, info, note); basket.append(box); hidden.append(hiddenId, hiddenQty);
    });
    validate();
  }

  function validate() {
    let bad = false;
    for (const { item, quantity } of cart.values()) {
      const val = decimalNumber(quantity);
      if (!Number.isFinite(val) || val < 0.0001 || val > item.available || val > 999999999) bad = true;
    }
    submit.disabled = bad || !cart.size || !source.value || !target.value || source.value === target.value;
    setError(bad ? 'Sepetteki miktarlar sıfırdan büyük ve mevcut stoktan az/eşit olmalıdır.' : '');
    return !submit.disabled;
  }

  form.addEventListener('submit', event => {
    if (!validate() || !confirm('Sepetteki tüm malzemeler hedef ambara transfer edilsin mi?')) event.preventDefault();
  });
  source.addEventListener('change', loadAvailable);
  target.addEventListener('change', loadAvailable);
  search.addEventListener('input', renderCatalog);
  refreshTargets();
  loadAvailable();
})();

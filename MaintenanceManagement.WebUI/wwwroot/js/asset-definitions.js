(() => {
  'use strict';

  // Kategori / marka / model kayıtlarının form üzerinden güncellenmesi.
  let editRequest = 0;
  function initEdit() {
    const buttons = document.querySelectorAll('.ad-edit');
    const cancel = document.getElementById('ad-cancel-edit');
    const formTitle = document.getElementById('ad-form-title');
    const message = document.getElementById('ad-edit-message');
    const showEdit = text => {
      formTitle.innerHTML = '<i class="fas fa-edit mr-2"></i>' + text;
      cancel.hidden = false;
      message.hidden = false;
      message.textContent = 'Düzenleme modundasınız. Değişiklikleri kaydetmek için aşağıdaki butona basın.';
    };
    const clear = () => {
      editRequest++;
      document.querySelectorAll('#categoryForm button[type="submit"]').forEach(b => b.disabled = false);
      ['categoryForm','brandForm','modelForm'].forEach(id => {
        const form = document.getElementById(id);
        if (!form) return;
        form.reset();
        const hiddenId = form.querySelector('input[name$=".Id"]');
        if (hiddenId) hiddenId.value = '0';
      });
      if (window.assetDefinitionChosenUnits) {
        window.assetDefinitionChosenUnits.clear();
        window.assetDefinitionRenderUnits?.();
      }
      formTitle.innerHTML = '<i class="fas fa-plus-circle mr-2"></i>Yeni Tanım';
      cancel.hidden = true;
      message.hidden = true;
      document.getElementById('ad-unit-error')?.replaceChildren();
    };
    cancel?.addEventListener('click', clear);
    buttons.forEach(button => button.addEventListener('click', async () => {
      const kind = button.dataset.kind;
      const form = document.getElementById(kind + 'Form');
      if (!form) return;
      const seq = ++editRequest;
      const id = form.querySelector('input[name$=".Id"]');
      const name = form.querySelector('input[name$=".Name"]');
      const active = form.querySelector('input[name$=".IsActive"][type="checkbox"]');
      if (id) id.value = button.dataset.id;
      if (name) name.value = button.dataset.name || '';
      if (active) active.checked = button.dataset.active === 'true';
      if (kind === 'model') {
        const brand = form.querySelector('select[name="Model.BrandId"]');
        if (brand) brand.value = button.dataset.brandId || '';
      }
      showEdit(kind === 'category' ? 'Kategori Düzenle' : kind === 'brand' ? 'Marka Düzenle' : 'Model Düzenle');
      form.querySelector('button[type="submit"]')?.scrollIntoView({ block: 'nearest' });
      if (kind === 'category') {
        const selected = window.assetDefinitionChosenUnits;
        selected?.clear();
        window.assetDefinitionRenderUnits?.();
        const url = new URL(form.dataset.unitsUrl, location.href);
        url.searchParams.set('id', button.dataset.id);
        message.textContent = 'Kategorinin kayıtlı organizasyonları yükleniyor...';
        form.querySelector('button[type="submit"]').disabled = true;
        try {
          const response = await fetch(url.toString(), { credentials: 'same-origin', headers: { Accept: 'application/json' } });
          if (!response.ok) throw new Error('Kayıtlı organizasyonlar yüklenemedi.');
          const ids = await response.json();
          if (seq !== editRequest) return;
          ids.forEach(value => selected.add(Number(value)));
          window.assetDefinitionRenderUnits?.();
          message.textContent = 'Kayıtlı organizasyonlar işaretlendi. İsterseniz kapsamı ve adı değiştirebilirsiniz.';
        } catch (error) {
          if (seq !== editRequest) return;
          message.textContent = error.message + ' Kayıtlı kapsam korunması için lütfen kaydetmeyin.';
          form.querySelector('button[type="submit"]').disabled = true;
          return;
        }
        form.querySelector('button[type="submit"]').disabled = false;
      }
      form.scrollIntoView({ behavior: 'smooth', block: 'start' });
      name?.focus({ preventScroll: true });
    }));
  }

  const filter = document.getElementById('ad-filter');
  filter?.addEventListener('input', () => {
    const q = filter.value.toLocaleLowerCase('tr-TR').trim();
    document.querySelectorAll('#ad-rows tr').forEach(row => {
      row.hidden = !row.textContent.toLocaleLowerCase('tr-TR').includes(q);
    });
  });
  const target = document.getElementById('ad-unit-tree');
  if (!target) { initEdit(); return; }
  const units = JSON.parse(document.getElementById('ad-units-json')?.textContent || '[]');
  const chosen = new Set();
  window.assetDefinitionChosenUnits = chosen;
  window.assetDefinitionRenderUnits = () => { sync(); render(); };
  const orderedUnits = [];
  const byParent = new Map();
  units.forEach(u => { const pid = u.parentId ?? 0; if (!byParent.has(pid)) byParent.set(pid, []); byParent.get(pid).push(u); });
  function walk(id, depth, visited = new Set()) {
    for (const u of (byParent.get(id) || [])) {
      if (visited.has(u.id)) continue;
      orderedUnits.push({ ...u, depth });
      const chain = new Set(visited); chain.add(u.id);
      walk(u.id, depth + 1, chain);
    }
  }
  walk(0,0);
  units.forEach(u => { if (!orderedUnits.some(x => x.id === u.id)) orderedUnits.push({...u,depth:0}); });
  const escapeHtml = s => String(s).replace(/[&<>"']/g, c => ({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[c]));
  const hidden = document.getElementById('ad-unit-hidden');
  function sync() {
    hidden.replaceChildren(...[...chosen].map(id => {
      const h = document.createElement('input'); h.type = 'hidden'; h.name = 'Category.UnitIds'; h.value = id; return h;
    }));
  }
  function render() {
    const q = document.getElementById('ad-unit-search').value.toLocaleLowerCase('tr-TR').trim();
    target.innerHTML = orderedUnits.filter(u => u.name.toLocaleLowerCase('tr-TR').includes(q)).map(u =>
      `<label class="ad-unit" style="margin-left:${Math.min(3,u.depth)*18}px"><input type="checkbox" value="${u.id}" ${chosen.has(u.id) ? 'checked' : ''} /> <i class="fas fa-building text-primary mr-1"></i> ${escapeHtml(u.name)}</label>`
    ).join('') || '<div class="text-muted p-2">Organizasyon bulunamadı.</div>';
    target.querySelectorAll('input[type=checkbox]').forEach(box => box.addEventListener('change', () => {
      const id = Number(box.value); box.checked ? chosen.add(id) : chosen.delete(id); sync();
    }));
  }
  document.getElementById('ad-unit-search').addEventListener('input', render);
  document.getElementById('categoryForm')?.addEventListener('submit', ev => {
    if (!chosen.size) { ev.preventDefault(); document.getElementById('ad-unit-error').textContent = 'En az bir organizasyon seçmelisiniz.'; }
  });
  render();
  initEdit();
})();

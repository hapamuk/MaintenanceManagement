(() => {
  'use strict';
  const root = document.getElementById('assetWizard');
  if (!root) return;
  const form = document.getElementById('assetWizardForm');
  const panels = [...document.querySelectorAll('.aw-panel')];
  const stepItems = [...document.querySelectorAll('.aw-step')];
  const prev = document.getElementById('aw-prev');
  const next = document.getElementById('aw-next');
  const submit = document.getElementById('aw-submit');
  let step = 0;
  const units = JSON.parse(document.getElementById('aw-unit-data').textContent || '[]');

  function esc(value) { return String(value ?? '').replace(/[&<>"']/g, c => ({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[c])); }
  function value(name) {
    const el = form.querySelector(`[name="Input.${name}"]:checked`) || form.querySelector(`[name="Input.${name}"]`);
    return el ? el.value : '';
  }
  function display(name) {
    const el = form.querySelector(`[name="Input.${name}"]:checked`) || form.querySelector(`[name="Input.${name}"]`);
    if (!el) return '';
    if (el.tagName === 'SELECT') return el.selectedOptions[0]?.textContent?.trim() || '';
    return el.value;
  }
  function active(stepNumber) {
    step = stepNumber;
    panels.forEach((p, i) => p.hidden = i !== step);
    stepItems.forEach((s, i) => { s.classList.toggle('active', i === step); s.classList.toggle('done', i < step); });
    prev.disabled = step === 0;
    next.hidden = step === 5;
    submit.hidden = step !== 5;
    document.getElementById('aw-step-hint').textContent = `Adım ${step + 1} / 6`;
    if (step === 5) preview();
    document.dispatchEvent(new CustomEvent('aw:step-change',{detail:{step}}));
  }
  function invalid(node, msg) { node.setCustomValidity(msg); node.reportValidity(); node.addEventListener('input', () => node.setCustomValidity(''), {once:true}); return false; }
  function validStep() {
    if (step <= 1) {
      const required = [...panels[step].querySelectorAll('input[required],select[required]')];
      for (const el of required) {
        if (!el.checkValidity()) { el.reportValidity(); return false; }
        if (el.tagName === 'SELECT' && (!el.value || el.selectedOptions[0]?.disabled)) return invalid(el, 'Bu alanı seçiniz.');
      }
    }
    if (step === 2) {
      for (const id of ['aw-responsible']) {
        const el = document.getElementById(id);
        if (!Number(el.value)) { document.getElementById('aw-tree-' + (id === 'aw-responsible' ? 'responsible':'using')).focus(); alert('Sorumlu organizasyonu ağaçtan seçiniz.'); return false; }
      }
    }
    return true;
  }
  prev.addEventListener('click', () => active(Math.max(step - 1, 0)));
  next.addEventListener('click', () => { if (validStep()) active(Math.min(step + 1, 5)); });
  form.addEventListener('submit', event => { if (step !== 5 || !validStep()) { event.preventDefault(); return; } submit.disabled = true; });

  function filterSelect(select, groupId, reset) {
    [...select.options].forEach(opt => { if (!opt.value) return; opt.hidden = !!opt.dataset.group && opt.dataset.group !== groupId; opt.disabled = opt.hidden; });
    if (reset && select.selectedOptions[0]?.disabled) select.value = '';
  }
  const cat = document.getElementById('aw-category');
  const brand = document.getElementById('aw-brand'), model = document.getElementById('aw-model');
  brand.addEventListener('change', () => { model.value = ''; filterSelect(model,brand.value,true); });
  filterSelect(model,brand.value,false);

  // Birim hiyerarşisi yalnızca merkezî Unit kayıtlarından kurulur.
  const unitById = new Map(units.map(u => [Number(u.id), u]));
  const isRoot = u => /^genel müdürlük$/i.test(u.text.trim()) || /daire(si)? başkanlığı$/i.test(u.text.trim());
  const selectable = units.filter(isRoot);
  const selectableIds = new Set(selectable.map(u => Number(u.id)));
  const branchState = { responsible: new Set(), using: new Set() };
  function selectedName(id) { return unitById.get(Number(id))?.text || '—'; }
  function path(id) { return selectedName(id); } // Ön izlemede yalnızca seçili organizasyon.
  function nearestVisibleParent(u) {
    let pid = Number(u.parentId || 0), visited = new Set();
    while (pid && !visited.has(pid)) {
      visited.add(pid);
      if (selectableIds.has(pid)) return pid;
      pid = Number(unitById.get(pid)?.parentId || 0);
    }
    return 0;
  }
  const branches = new Map();
  const byParent = new Map();
  selectable.forEach(u => {
    const parentId = nearestVisibleParent(u); branches.set(Number(u.id), parentId);
    if (!byParent.has(parentId)) byParent.set(parentId, []);
    byParent.get(parentId).push(u);
  });
  const childUnits = new Map();
  units.forEach(u => { const p=Number(u.parentId || 0); if (!childUnits.has(p)) childUnits.set(p, []); childUnits.get(p).push(Number(u.id)); });
  function descendantIds(unitId) {
    const found = new Set(), todo = [Number(unitId)];
    while (todo.length) {
      const id = todo.pop(); if (!id || found.has(id)) continue;
      found.add(id); (childUnits.get(id) || []).forEach(x=>todo.push(x));
    }
    return found;
  }
  function renderTree(which, query='') {
    const container = document.getElementById('aw-tree-' + which);
    const hidden = document.getElementById('aw-' + which);
    const q = query.toLocaleLowerCase('tr-TR').trim();
    const matches = new Set();
    if (q) selectable.forEach(u => { if (u.text.toLocaleLowerCase('tr-TR').includes(q)) {
      matches.add(Number(u.id)); let pid = branches.get(Number(u.id));
      while (pid && !matches.has(pid)) { matches.add(pid); pid=branches.get(pid); }
    }});
    function render(id, level) {
      return (byParent.get(id) || []).filter(u => !q || matches.has(Number(u.id))).map(u => {
        const uid=Number(u.id), children=(byParent.get(uid)||[]).some(c=>!q || matches.has(Number(c.id)));
        const expanded = q ? true : branchState[which].has(uid);
        const selected = Number(hidden.value) === uid;
        return `<div class="aw-branch" role="treeitem" aria-expanded="${children ? expanded : 'false'}">
          <div class="aw-branch-line" style="padding-left:${Math.min(level,10)*18+7}px">
            ${children ? `<button type="button" class="aw-tree-toggle" data-toggle="${uid}" aria-label="${esc(u.text)} dalını ${expanded?'kapat':'aç'}"><i class="fas fa-chevron-${expanded?'down':'right'}"></i></button>` : `<span class="aw-tree-toggle-spacer"></span>`}
            <button type="button" class="aw-tree-choice ${selected?'selected':''}" data-select="${uid}" aria-selected="${selected}"><i class="fas fa-building mr-2"></i>${esc(u.text)}</button>
          </div>${children && expanded ? `<div role="group">${render(uid,level+1)}</div>` : ''}
        </div>`;
      }).join('');
    }
    container.innerHTML = render(0,0) || '<div class="p-3 text-muted">Organizasyon bulunamadı.</div>';
    container.querySelectorAll('[data-toggle]').forEach(b=>b.addEventListener('click',()=>{
      const id=Number(b.dataset.toggle), state=branchState[which];
      state.has(id)?state.delete(id):state.add(id); renderTree(which,query);
    }));
    container.querySelectorAll('[data-select]').forEach(b=>b.addEventListener('click',()=>{
      hidden.value=b.dataset.select;
      document.getElementById('aw-picked-'+which).innerHTML=`<i class="fas fa-check-circle"></i> ${esc(selectedName(hidden.value))}`;
      renderTree(which,query);
      if (which==='using') refreshAssignees(true);
    }));
    document.getElementById('aw-picked-'+which).innerHTML = hidden.value
      ? `<i class="fas fa-check-circle"></i> ${esc(selectedName(hidden.value))}`
      : (which === 'using' ? '<i class="fas fa-info-circle"></i> Seçilmedi (isteğe bağlı)' : '<i class="fas fa-info-circle"></i> Henüz organizasyon seçilmedi');
  }
  document.querySelectorAll('.aw-tree-search').forEach(el => el.addEventListener('input',()=>renderTree(el.dataset.target,el.value)));
  document.querySelectorAll('.aw-expand-all').forEach(b=>b.addEventListener('click',()=>{
    branchState[b.dataset.target]=new Set(selectableIds); const search=document.querySelector(`.aw-tree-search[data-target="${b.dataset.target}"]`);
    renderTree(b.dataset.target,search.value);
  }));
  document.querySelectorAll('.aw-collapse-all').forEach(b=>b.addEventListener('click',()=>{
    branchState[b.dataset.target].clear(); const search=document.querySelector(`.aw-tree-search[data-target="${b.dataset.target}"]`);
    search.value=''; renderTree(b.dataset.target);
  }));
  // Varsayılan olarak Genel Müdürlük açık, daireler kapalı.
  selectable.filter(u => nearestVisibleParent(u)===0).forEach(u => {branchState.responsible.add(Number(u.id));branchState.using.add(Number(u.id));});
  renderTree('responsible');renderTree('using');
  document.getElementById('aw-clear-using')?.addEventListener('click', () => {
    const hidden = document.getElementById('aw-using');
    hidden.value = '';
    document.getElementById('aw-picked-using').innerHTML = '<i class="fas fa-info-circle"></i> Seçilmedi (isteğe bağlı)';
    renderTree('using', document.querySelector('.aw-tree-search[data-target="using"]')?.value || '');
    refreshAssignees(true);
  });

  // Kullanan organizasyon ve tüm alt birimlerindeki aktif kullanıcılar.
  const assignee=document.getElementById('aw-assignee');
  function refreshAssignees(reset) {
    const selectedUnit=Number(document.getElementById('aw-using').value || 0);
    const eligible=selectedUnit ? descendantIds(selectedUnit) : new Set();
    [...assignee.options].forEach(o=>{
      if (!o.value) return;
      const allowed=eligible.has(Number(o.dataset.group));
      o.hidden=!allowed; o.disabled=!allowed;
    });
    if (reset || assignee.selectedOptions[0]?.disabled) assignee.value='';
    assignee.disabled = !selectedUnit;
    document.getElementById('aw-assignee-hint').textContent=selectedUnit
      ? `${selectedName(selectedUnit)} ve alt birimlerine bağlı aktif personeller görüntüleniyor.`
      : 'Önce Kullanılan Organizasyonu seçin.';
  }
  refreshAssignees(false);

  // V6.4 — Kayıtlı binalara göre isteğe bağlı il / ilçe / mahalle arama.
  // Dış geocoding servisi yok: yalnızca veritabanında tanımlanan adres etiketlerini kullanır.
  const building=document.getElementById('aw-building'),floor=document.getElementById('aw-floor'),room=document.getElementById('aw-room');
  const locationInput=document.getElementById('aw-location');
  const mapMessage=document.getElementById('aw-map-message');
  const config=JSON.parse(document.getElementById('aw-places-config')?.textContent||'{}');
  const province=document.getElementById('aw-province');
  const district=document.getElementById('aw-district');
  const neighborhood=document.getElementById('aw-neighborhood');
  const filterCount=document.getElementById('aw-filter-count');
  let buildings=[],map=null,markers=[],buildingsLoaded=false, mapTilesFailed=false;
  const markerByBuilding=new Map();
  const normalize=t=>String(t??'').trim().toLocaleLowerCase('tr-TR');
  const hasCoordinates=b=>Number.isFinite(Number(b.latitude))&&Number.isFinite(Number(b.longitude))&&
      Number(b.latitude)>=-90&&Number(b.latitude)<=90&&Number(b.longitude)>=-180&&Number(b.longitude)<=180;
  function replaceOptions(el,list,placeholder) {
    el.replaceChildren(new Option(placeholder,''));
    list.forEach(x=>el.add(new Option(x.name,String(x.id))));
    el.disabled=!list.length;
  }
  const regions=window.AssetRegions;
  async function initArea(){
    regions.setProvince(province);
    try{await regions.loadDistrictSelect(district);}catch(error){mapMessage.textContent=error.message;}
    regions.setOptions(neighborhood,[],'Önce ilçe seçiniz');
  }
  async function selectAreaForBuilding(b){
    if(!b?.province || regions.cmp(b.province)!==regions.cmp('Kocaeli'))return;
    province.value='Kocaeli';
    try{
      await regions.loadDistrictSelect(district,b.district||'');
      await regions.loadNeighborhoodSelect(neighborhood,district,b.neighborhood||'');
    }catch(error){mapMessage.textContent=error.message;}
  }
  async function onAreaChanged(which){
    try{
      if(which==='province'){
        await regions.loadDistrictSelect(district);
        regions.setOptions(neighborhood,[],'Önce ilçe seçiniz');
      } else if(which==='district'){
        await regions.loadNeighborhoodSelect(neighborhood,district);
      }
    }catch(error){mapMessage.textContent=error.message;}
    applyFilters(false);
    if(map && (district.value||neighborhood.value)) {
      try {const found=await regions.focus(map,province.value,district.value,neighborhood.value);
        if(!found)mapMessage.textContent='Bölge merkezi bulunamadı; haritayı fareyle kullanabilirsiniz.';}
      catch(error){mapMessage.textContent=error.message;}
    } else if(map)focusBuildings(scopedBuildings());
  }
  function scopedBuildings(){
    return buildings.filter(b=>(!province.value||normalize(b.province)===normalize(province.value))&&
      (!district.value||normalize(b.district)===normalize(district.value))&&
      (!neighborhood.value||normalize(b.neighborhood)===normalize(neighborhood.value)));
  }
  function focusBuildings(list,animate=true) {
    if(!map)return;
    const valid=list.filter(hasCoordinates);
    if(valid.length===1){map.setView([Number(valid[0].latitude),Number(valid[0].longitude)],17,{animate});return;}
    if(valid.length>1){map.fitBounds(L.latLngBounds(valid.map(b=>[Number(b.latitude),Number(b.longitude)])),
      {padding:[44,44],maxZoom:16,animate});return;}
    // Kullanıcı adresi olmayan bir grubu filtrelemişse sahte koordinata uçmayız.
    if(!buildings.length)map.setView(regions.center,10,{animate:false});
  }
  function applyFilters(focus=true){
    const shown=scopedBuildings();const ids=new Set(shown.map(b=>Number(b.id)));
    markers.forEach(({item,marker})=>{
      if(ids.has(Number(item.id))) {
        if(!map.hasLayer(marker))marker.addTo(map);
      } else if(map.hasLayer(marker))map.removeLayer(marker);
    });
    // Bina alanında sadece filtredeki binalar olsun; seçili kaydı kaldırma.
    const selected=building.value;
    replaceOptions(building,shown,'Bina seçiniz...');
    if(shown.some(b=>String(b.id)===selected))building.value=selected;
    else if(selected){chooseBuilding('',false);}
    filterCount.textContent=`${shown.length} / ${buildings.length} kayıtlı bina gösteriliyor.`;
    if(focus)focusBuildings(shown);
    if(map)requestAnimationFrame(()=>map.invalidateSize({pan:false}));
  }
  function chooseBuilding(id,fly=true) {
    const b=buildings.find(x=>x.id===Number(id));
    if(b && ![...building.options].some(o=>o.value===String(b.id))){
      regions.setProvince(province); district.value=''; neighborhood.value=''; applyFilters(false);
    }
    building.value=b?String(b.id):'';
    replaceOptions(floor,b?.floors||[],'Kat seçiniz...');
    replaceOptions(room,[],'Önce kat seçiniz...');
    locationInput.value='';
    document.getElementById('aw-location-error').textContent='';
    if(b&&map&&fly)map.flyTo([Number(b.latitude),Number(b.longitude)],Math.max(map.getZoom(),17));
    markers.forEach(({item,marker})=>marker.setOpacity(!b||item.id===b.id?1:0.5));
  }
  building.addEventListener('change',()=>chooseBuilding(building.value));
  floor.addEventListener('change',()=>{
    const b=buildings.find(x=>x.id===Number(building.value));
    const f=b?.floors?.find(x=>x.id===Number(floor.value));
    replaceOptions(room,f?.rooms||[],'Oda seçiniz...');locationInput.value='';
  });
  room.addEventListener('change',()=>{
    const b=buildings.find(x=>x.id===Number(building.value));
    const f=b?.floors?.find(x=>x.id===Number(floor.value));
    const r=f?.rooms?.find(x=>x.id===Number(room.value));
    locationInput.value=r?.siteLocationId?String(r.siteLocationId):'';
    document.getElementById('aw-location-error').textContent=r&&!r.siteLocationId?
      'Bu oda için henüz bir SiteLocation kaydı yok. Demirbaş Yeri Tanıtımı ekranından odayı düzenleyin.':'';
  });
  province.addEventListener('change',()=>onAreaChanged('province'));
  district.addEventListener('change',()=>onAreaChanged('district'));
  neighborhood.addEventListener('change',()=>onAreaChanged('neighborhood'));
  document.getElementById('aw-filter-clear')?.addEventListener('click',()=>{
    regions.setProvince(province);district.value='';regions.setOptions(neighborhood,[],'Önce ilçe seçiniz');applyFilters(true);
  });
  function renderMap(){
    const host=document.getElementById('aw-building-map');
    if(!host)return;
    if(!window.L){mapMessage.textContent='Leaflet yüklenemedi. Bina listesinden seçim yapabilirsiniz.';return;}
    if(map){map.invalidateSize({pan:false});return;}
    if(host.getClientRects().length===0||host.clientWidth===0)return;
    map=L.map(host,{scrollWheelZoom:true,zoomControl:true,zoomSnap:0.5,zoomDelta:0.5,
      wheelPxPerZoomLevel:90,preferCanvas:true});
    const tiles=L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png',{
      maxZoom:19,attribution:'&copy; <a href="https://www.openstreetmap.org/copyright" target="_blank" rel="noopener">OpenStreetMap katkıda bulunanlar</a>'
    }).addTo(map);
    tiles.on('tileerror',()=>{
      if(!mapTilesFailed){mapTilesFailed=true;mapMessage.textContent='Harita döşemeleri yüklenemedi; internet bağlantısını kontrol edin. Kayıtlı bina listesi kullanılabilir.';}
    });
    buildings.forEach(b=>{
      if(!hasCoordinates(b))return;
      const marker=L.marker([Number(b.latitude),Number(b.longitude)],{title:b.name});
      marker.bindTooltip(esc(b.name),{direction:'top'});
      marker.on('click',async()=>{
        chooseBuilding(b.id,true);await selectAreaForBuilding(b);
        if(building.value===String(b.id))applyFilters(false);
      });
      markers.push({item:b,marker});markerByBuilding.set(b.id,marker);
    });
    applyFilters(false);focusBuildings(scopedBuildings(),false);
    if(!buildings.length)map.setView(regions.center,10);
    for(const delay of [100,300,650])setTimeout(()=>map?.invalidateSize({pan:false}),delay);
  }
  async function loadBuildings(){
    if(!config.apiUrl){mapMessage.textContent='Bina API adresi bulunamadı.';return;}
    try {
      const response=await fetch(config.apiUrl,{credentials:'same-origin',headers:{Accept:'application/json'}});
      if(!response.ok)throw new Error('Bina ve oda listesi alınamadı.');
      const raw=await response.json();buildings=Array.isArray(raw)?raw:[];
      const saved=String(locationInput.value||'');
      await initArea();
      replaceOptions(building,buildings,'Bina seçiniz...');
      mapMessage.textContent=buildings.length?`${buildings.length} kayıtlı bina mevcut. Haritada bina işaretine veya listeden bina adına tıklayın.`:
        'Henüz bina kaydı bulunamadı. Demirbaş Yeri Tanıtımı ekranından bina ekleyin.';
      buildingsLoaded=true;
      if(step===3)showMapWhenVisible();
      if(saved){
        outer: for(const b of buildings)for(const f of b.floors||[])for(const r of f.rooms||[]){
          if(String(r.siteLocationId)===saved){
            chooseBuilding(b.id,false);await selectAreaForBuilding(b);applyFilters(false);floor.value=String(f.id);
            replaceOptions(room,f.rooms||[],'Oda seçiniz...');room.value=String(r.id);
            locationInput.value=saved;break outer;
          }
        }
      }
    }catch(error){mapMessage.textContent=error.message;buildingsLoaded=false;}
  }
  function showMapWhenVisible(){
    if(step!==3||!buildingsLoaded)return;
    requestAnimationFrame(()=>requestAnimationFrame(()=>{
      renderMap();map?.invalidateSize({pan:false});
      setTimeout(()=>map?.invalidateSize({pan:false}),220);
    }));
  }
  document.addEventListener('aw:step-change',showMapWhenVisible);
  document.getElementById('aw-map-fit')?.addEventListener('click',()=>{
    // Tüm binaları göster: il/ilçe/mahalle filtresini de kaldır.
    regions.setProvince(province);district.value='';regions.setOptions(neighborhood,[],'Önce ilçe seçiniz');applyFilters(true);
  });
  window.addEventListener('resize',()=>{if(step===3&&map)map.invalidateSize({pan:false});});
  loadBuildings();

  function preview() {
    const status=({Active:'Aktif',Passive:'Pasif',Scrap:'Hurda'})[value('Status')]||value('Status');
    const selectedRoom=room.value ? `${building.selectedOptions[0]?.textContent} / ${floor.selectedOptions[0]?.textContent} / ${room.selectedOptions[0]?.textContent}` : 'Seçilmedi';
    const cards=[
      ['Kimlik Bilgileri','fa-id-card',[['Demirbaş Kodu','Kayıt sonrası otomatik'],['Demirbaş Adı',value('Name')],['Barkod',value('BarcodeNumber')]]],
      ['Sınıflandırma','fa-layer-group',[['Kategori',display('AssetCategoryId')],['Marka',display('BrandId')],['Model',display('AssetModelId')]]],
      ['Organizasyon ve Konum','fa-sitemap',[['Sorumlu Organizasyon',selectedName(value('ResponsibleUnitId'))],['Kullanılan Organizasyon',value('UsingUnitId') ? selectedName(value('UsingUnitId')) : 'Seçilmedi'],['Bina / Kat / Oda',selectedRoom]]],
      ['Zimmet ve Durum','fa-user-check',[['İlk Zimmet',value('AssigneeUserId')?display('AssigneeUserId'):'Zimmet yok'],['Durum',status],['Açıklama',value('Description')]]]
    ];
    document.getElementById('aw-preview').innerHTML=`<div class="aw-preview-hero"><i class="fas fa-laptop"></i><div><small>Yeni Demirbaş Kaydı</small><h3>${esc(value('Name')||'Demirbaş')}</h3><span class="aw-preview-status">${esc(status)}</span></div></div><div class="aw-preview-grid">${cards.map(([title,icon,items])=>`<div class="aw-preview-card"><h5><i class="fas ${icon}"></i>${esc(title)}</h5>${items.map(([k,v])=>`<div class="aw-preview-line"><span>${esc(k)}</span><strong>${esc(v||'—')}</strong></div>`).join('')}</div>`).join('')}</div>`;
  }
  active(0);
})();

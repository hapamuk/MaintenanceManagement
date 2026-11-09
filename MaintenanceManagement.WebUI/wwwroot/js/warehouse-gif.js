(function () {
 'use strict';
 const root=document.getElementById('warehouseWizardV2'); if(!root)return;
 const form=document.getElementById('wh2Form');
 const panes=[...root.querySelectorAll('[data-stage]')];
 const steps=[...root.querySelectorAll('[data-step-mark]')];
 const asideSteps=[...root.querySelectorAll('[data-aside-step]')];
 const unit=document.getElementById('wh2UnitId'), name=document.getElementById('wh2Name');
 const code=document.getElementById('wh2Code'), location=document.getElementById('wh2Location'), description=document.getElementById('wh2Description');
 const hidden=document.getElementById('wh2SelectedHidden'), available=document.getElementById('wh2AvailablePeople'), chosen=document.getElementById('wh2SelectedPeople');
 const errorBox=document.getElementById('wh2Error');
 const orgs=new Map([...root.querySelectorAll('[data-org-option]')].map(x=>[String(x.dataset.unitId),x.dataset.unitName]));
 const initialSelected=JSON.parse(root.dataset.selectedIds || '[]').map(String);
 let step=0, maxUnlocked=0, candidates=new Map(), selected=new Set(initialSelected), personnelLoadedFor=null, token=0, personLoadError=false;
 const isEdit=root.dataset.isEdit==='true';
 const statusInput=document.getElementById('wh2IsActive');
 const statusBadge=document.getElementById('wh2StatusBadge');
 const statusBox=document.getElementById('wh2StatusBox');
 const statusHelp=document.getElementById('wh2StatusHelp');
 function updateStatus(){
  if(!statusInput)return;
  const active=statusInput.checked;
  if(statusBadge){statusBadge.textContent=active?'Aktif':'Pasif';statusBadge.classList.toggle('is-active',active);statusBadge.classList.toggle('is-passive',!active);}
  if(statusBox){statusBox.classList.toggle('is-active',active);statusBox.classList.toggle('is-passive',!active);}
  if(statusHelp)statusHelp.textContent=active?'Ambar aktif olarak oluşturulacaktır.':'Ambar pasif olarak oluşturulacaktır.';
  root.querySelectorAll('[data-status-preview]').forEach(el=>el.textContent=active?'Aktif':'Pasif');
 }
 if(statusInput)statusInput.addEventListener('change',()=>{updateStatus();preview();});
 const enc=s=>String(s??'').replace(/[&<>"']/g,c=>({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[c]));
 const initials=s=>(String(s||'').trim().split(/\s+/).filter(Boolean).slice(0,2).map(v=>v[0]).join('')||'?').toLocaleUpperCase('tr');
 function message(s){errorBox.textContent=s||'';errorBox.hidden=!s;if(s)errorBox.scrollIntoView({block:'nearest',behavior:'smooth'});}
 function label(x){return x.unitName || 'Birim adı bulunamadı';}
 function preview(){
  const fields={'[data-code-preview]':code.value,'[data-name-preview]':name.value||'—','[data-location-preview]':location.value||'—','[data-description-preview]':description.value||'—','[data-org-preview]':orgs.get(String(unit.value))||'Henüz seçilmedi'};
  Object.entries(fields).forEach(([sel,value])=>root.querySelectorAll(sel).forEach(e=>e.textContent=value));
  if(statusInput)root.querySelectorAll('[data-status-preview]').forEach(e=>e.textContent=statusInput.checked?'Aktif':'Pasif');
  root.querySelectorAll('[data-selected-count]').forEach(e=>e.textContent=selected.size);
  root.querySelectorAll('[data-available-count]').forEach(e=>e.textContent=[...candidates.keys()].filter(id=>!selected.has(id)).length);
  if(step===3){const list=document.getElementById('wh2PreviewPeople');list.innerHTML=[...selected].map(id=>{const p=candidates.get(id);return p?`<div class="wh2-review-person"><span class="wh2-avatar">${enc(initials(p.name))}</span><span><b>${enc(p.name)}</b><small>${enc(label(p))}</small></span></div>`:''}).join('');}
 }
 function row(p,isSelected){return `<div class="wh2-person"><span class="wh2-avatar">${enc(initials(p.name))}</span><div class="wh2-person-info"><strong>${enc(p.name)}</strong><small title="${enc(label(p))}">${enc(label(p))}</small></div>${isSelected?'<span class="wh2-authorized"><i class="fas fa-check"></i> Yetkili</span>':''}<button type="button" class="wh2-icon-button ${isSelected?'wh2-remove':'wh2-add'}" data-${isSelected?'remove':'add'}="${enc(p.id)}" aria-label="${isSelected?'Personelin yetkisini kaldır':'Personeli yetkilendir'}"><i class="fas fa-${isSelected?'trash-alt':'plus'}"></i></button></div>`;}
 function renderPeople(){
  const q=(document.getElementById('wh2PeopleSearch').value||'').trim().toLocaleLowerCase('tr');
  const list=[...candidates.values()].filter(p=>!selected.has(String(p.id))&&(p.name+' '+label(p)).toLocaleLowerCase('tr').includes(q));
  available.innerHTML=list.length?list.map(p=>row(p,false)).join(''):'<div class="wh2-empty">'+(personLoadError?'Personeller yüklenemedi. Organizasyonu yeniden seçin.': 'Listelenecek personel bulunamadı.')+'</div>';
  const selectedRows=[...selected].map(id=>candidates.get(id)).filter(Boolean);
  chosen.innerHTML=selectedRows.length?selectedRows.map(p=>row(p,true)).join(''):'<div class="wh2-empty">Henüz yetkili personel seçilmedi.</div>';
  hidden.replaceChildren(...[...selected].map(id=>{const input=document.createElement('input');input.type='hidden';input.name='SelectedPersonnelIds';input.value=id;return input;}));
  preview();
 }
 function selectOrg(id){
  const previous=String(unit.value||''); unit.value=id;
  root.querySelectorAll('[data-org-option]').forEach(el=>{const checked=String(el.dataset.unitId)===String(id);el.classList.toggle('is-selected',checked);el.setAttribute('aria-checked',checked?'true':'false');});
  if(previous!==String(id)){selected.clear();candidates.clear();personnelLoadedFor=null;maxUnlocked=Math.min(maxUnlocked,2);renderPeople();}
  preview();
 }
 async function loadPeople(){
  const id=String(unit.value||''); if(!id)return;
  if(personnelLoadedFor===id)return;
  const req=++token;personLoadError=false;
  available.innerHTML='<div class="wh2-empty"><i class="fas fa-spinner fa-spin"></i> Personeller yükleniyor...</div>';
  chosen.innerHTML='<div class="wh2-empty">Seçimler hazırlanıyor...</div>';
  try{
    const response=await fetch(root.dataset.personnelUrl+'?unitId='+encodeURIComponent(id),{credentials:'same-origin',headers:{Accept:'application/json'}});
    if(!response.ok)throw new Error('Personel sorgusu başarısız.');
    const data=await response.json();if(req!==token)return;
    candidates=new Map(data.map(x=>[String(x.id),{id:String(x.id),name:String(x.name||''),unitName:String(x.unitName||''),unitId:x.unitId}]));
    selected=new Set([...selected].filter(i=>candidates.has(i)));personnelLoadedFor=id;
  }catch(e){if(req!==token)return;candidates.clear();personLoadError=true;message('Personeller alınamadı. Bağlantıyı ve organizasyon seçimini kontrol edin.');}
  renderPeople();
 }
 function complete(n){
  if(n===0){if(!name.value.trim()){message('Ambar adını giriniz.');name.focus();return false;}if(name.value.trim().length>150){message('Ambar adı en fazla 150 karakter olabilir.');return false;}if(location.value.length>250||description.value.length>500){message('Konum veya açıklama metni fazla uzun.');return false;}return true;}
  if(n===1){if(!unit.value || !orgs.has(String(unit.value))){message('Genel Müdürlük veya Daire Başkanlığı seçiniz.');return false;}return true;}
  if(n===2){if(personLoadError){message('Personel listesi alınamadı.');return false;}if(selected.size<1){message('En az bir personel yetkilendiriniz.');return false;}return true;}
  return true;
 }
 function show(n){
  step=n;maxUnlocked=Math.max(maxUnlocked,n);message('');
  panes.forEach((p,i)=>{p.hidden=i!==n;p.classList.toggle('is-active',i===n);});
  steps.forEach((s,i)=>{const done=i<n;s.classList.toggle('is-done',done);s.classList.toggle('is-active',i===n);s.querySelector('.wh2-step-circle').innerHTML=done?'<i class="fas fa-check"></i>':String(i+1);});
  asideSteps.forEach((s,i)=>{s.classList.toggle('is-done',i<n);s.classList.toggle('is-active',i===n);s.querySelector('.wh2-aside-index').innerHTML=i<n?'<i class="fas fa-check"></i>':String(i+1);});
  document.getElementById('wh2Prev').hidden=n===0;
  document.getElementById('wh2Next').hidden=n===3;
  document.getElementById('wh2Save').hidden=n!==3;
  if(n===2)loadPeople();
  preview();
 }
 document.getElementById('wh2Next').addEventListener('click',()=>{if(!complete(step))return;show(Math.min(3,step+1));});
 document.getElementById('wh2Prev').addEventListener('click',()=>show(Math.max(0,step-1)));
 root.querySelectorAll('[data-org-option]').forEach(el=>el.addEventListener('click',()=>selectOrg(String(el.dataset.unitId))));
 document.getElementById('wh2OrgSearch').addEventListener('input',e=>{const q=e.target.value.trim().toLocaleLowerCase('tr');root.querySelectorAll('[data-org-option]').forEach(el=>el.hidden=!el.dataset.unitName.toLocaleLowerCase('tr').includes(q));});
 document.getElementById('wh2PeopleSearch').addEventListener('input',renderPeople);
 root.addEventListener('click',e=>{const add=e.target.closest('[data-add]'),remove=e.target.closest('[data-remove]');if(add){selected.add(String(add.dataset.add));renderPeople();}if(remove){selected.delete(String(remove.dataset.remove));renderPeople();}});
 [name,code,location,description].forEach(e=>e.addEventListener('input',preview));
 form.addEventListener('submit',e=>{for(let i=0;i<3;i++){if(!complete(i)){e.preventDefault();show(i);complete(i);return;}}if(step!==3){e.preventDefault();show(3);return;}renderPeople();});
 // Form-validation failures preserve initial selections; start from the first stage for clarity.
 if(unit.value)selectOrg(String(unit.value));
 updateStatus();renderPeople();show(0);
})();

/* Demirbaş Yeri Tanıtımı: ilçe/mahalle ile harita odağı; haritadan nokta seçince adres bulma */
(() => {
'use strict';
const r=window.AssetRegions;if(!r)return;
const byId=id=>document.getElementById(id);
const cr={p:byId('buildingProvince'),d:byId('buildingDistrict'),n:byId('buildingNeighborhood')};
const ed={p:byId('edit-building-province'),d:byId('edit-building-district'),n:byId('edit-building-neighborhood')};
const msg=byId('place-map-message');
let createRequest=0,editRequest=0;
async function fill(bundle,values){
 const request= bundle===cr?++createRequest:++editRequest;
 r.setProvince(bundle.p,'Kocaeli');
 try {
  await r.loadDistrictSelect(bundle.d,values?.district||'');
  if(request!==(bundle===cr?createRequest:editRequest))return;
  await r.loadNeighborhoodSelect(bundle.n,bundle.d,values?.neighborhood||'');
 } catch(error) {msg.textContent='Adres listesi yüklenemedi: '+error.message;}
}
async function focusArea(bundle){
 const map=window.assetPlaceAdminMap;if(!map)return;
 try {await r.focus(map,'Kocaeli',bundle.d.value,bundle.n.value);}
 catch(error){msg.textContent='Bölge odağı alınamadı: '+error.message;}
}
function wire(bundle){
 bundle.d?.addEventListener('change',async()=>{
  try{await r.loadNeighborhoodSelect(bundle.n,bundle.d);await focusArea(bundle);}
  catch(error){msg.textContent=error.message;}
 });
 bundle.n?.addEventListener('change',()=>focusArea(bundle));
}
wire(cr);wire(ed);fill(cr);fill(ed);
async function applyReverse(bundle,lat,lng){
 try {
  msg.textContent='Koordinata ait adres araştırılıyor...';
  const addr=await r.reverse(lat,lng);
  if(r.cmp(addr.province)!==r.cmp('Kocaeli')) {
   msg.textContent='Bu nokta Kocaeli dışında görünüyor veya adres tespit edilemedi. Koordinatı kontrol edin.';
   return;
  }
  await fill(bundle,addr);
  msg.textContent='Adres tespit edildi: '+[addr.province,addr.district,addr.neighborhood].filter(Boolean).join(' / ')+'. Noktayı kontrol ederek kaydedin.';
 }catch(error){msg.textContent='Adres otomatik tespit edilemedi: '+error.message+'. İlçe/mahalle seçerek devam edebilirsiniz.';}
}
window.addEventListener('assetplace:map-click',ev=>applyReverse(cr,ev.detail.lat,ev.detail.lng));
window.addEventListener('assetplace:edit-building',ev=>fill(ed,ev.detail));
byId('edit-building-detect')?.addEventListener('click',()=>{
 const lat=Number(byId('edit-building-lat')?.value),lng=Number(byId('edit-building-lng')?.value);
 if(!Number.isFinite(lat)||!Number.isFinite(lng)||!lat||!lng){msg.textContent='Önce kayıtlı bina seçin.';return;}
 applyReverse(ed,lat,lng);
});
})();

(() => {
 'use strict';
 const node=document.getElementById('place-admin-config');if(!node)return;
 const cfg=JSON.parse(node.textContent||'{}');
 const bsel=document.getElementById('place-building'),fsel=document.getElementById('place-floor');
 const lat=document.getElementById('buildingLat'),lng=document.getElementById('buildingLng');
 const list=document.getElementById('place-admin-list');
 const message=document.getElementById('place-map-message');
 let data=[], all=[], map=null, draftMarker=null;
 function options(select,rows,label){if(!select)return;select.replaceChildren(new Option(label,''));rows.forEach(x=>select.add(new Option(x.name,String(x.id))));}
 function e(id){return document.getElementById(id);}
 function safeNumber(n){return Number.isFinite(n)?Number(n).toFixed(7):'';}
 function renderMap(){
   const host=e('place-admin-map');if(!window.L){message.textContent='Leaflet yüklenemedi. Bağlantınızı kontrol edin; koordinatları elle girebilirsiniz.';return;}
   if(map)map.remove();
   map=L.map(host,{scrollWheelZoom:true,zoomControl:true});window.assetPlaceAdminMap=map;
   L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png',{
     maxZoom:19,attribution:'&copy; <a href="https://www.openstreetmap.org/copyright" target="_blank" rel="noopener">OpenStreetMap katkıda bulunanlar</a>'
   }).addTo(map);
   const bounds=[];
   data.forEach(b=>{
     if(!Number.isFinite(b.latitude)||!Number.isFinite(b.longitude))return;
     bounds.push([b.latitude,b.longitude]);
     L.marker([b.latitude,b.longitude]).bindPopup(b.name).addTo(map).on('click',()=>{
       bsel.value=String(b.id);syncFloors(); map.setView([b.latitude,b.longitude],17);
     });
   });
   if(bounds.length===1)map.setView(bounds[0],16);
   else if(bounds.length>1)map.fitBounds(bounds,{padding:[30,30],maxZoom:16});
   else map.setView([40.7654,29.9408],10); // Genel Müdürlüğün gerçek konumu bilinmediğinden ülke genel görünümü.
   map.on('click',event=>{
     lat.value=safeNumber(event.latlng.lat);lng.value=safeNumber(event.latlng.lng);
     if(draftMarker)map.removeLayer(draftMarker);
     draftMarker=L.marker(event.latlng).addTo(map).bindPopup('Yeni bina konumu').openPopup();
     window.dispatchEvent(new CustomEvent('assetplace:map-click',{detail:{lat:event.latlng.lat,lng:event.latlng.lng}}));
   });
   setTimeout(()=>map.invalidateSize(),140);
 }
 function syncFloors(){const b=data.find(x=>String(x.id)===bsel.value);options(fsel,b?.floors||[],'Kat seçiniz...');}
 function buildEdit(){
   options(e('edit-building-id'),all,'Bina seçiniz...');
   const floors=all.flatMap(b=>(b.floors||[]).map(f=>({...f,label:`${b.name} / ${f.name}`})));
   const rooms=all.flatMap(b=>(b.floors||[]).flatMap(f=>(f.rooms||[]).map(r=>({...r,label:`${b.name} / ${f.name} / ${r.name}`}))));
   const fs=e('edit-floor-id'),rs=e('edit-room-id');
   fs.replaceChildren(new Option('Kat seçiniz...',''));
   floors.forEach(f=>fs.add(new Option(f.label,String(f.id))));
   rs.replaceChildren(new Option('Oda seçiniz...',''));
   rooms.forEach(r=>rs.add(new Option(r.label,String(r.id))));
   e('edit-building-id').addEventListener('change',()=>{
      const b=all.find(x=>String(x.id)===e('edit-building-id').value);
      e('edit-building-name').value=b?.name||'';
      e('edit-building-lat').value=b?safeNumber(b.latitude):'';
      e('edit-building-lng').value=b?safeNumber(b.longitude):'';
      e('edit-building-province').value=b?.province||'';
      e('edit-building-district').value=b?.district||'';
      e('edit-building-neighborhood').value=b?.neighborhood||'';
      window.dispatchEvent(new CustomEvent('assetplace:edit-building',{detail:b||{}}));
      e('edit-building-active').checked=!!b?.isActive;
      if(b && map)map.setView([b.latitude,b.longitude],16);
   });
   fs.addEventListener('change',()=>{
      const f=floors.find(x=>String(x.id)===fs.value);
      e('edit-floor-name').value=f?.name||'';e('edit-floor-active').checked=!!f?.isActive;
   });
   rs.addEventListener('change',()=>{
      const r=rooms.find(x=>String(x.id)===rs.value);
      e('edit-room-name').value=r?.name||'';
      e('edit-room-location').value=r?.siteLocationId?String(r.siteLocationId):'';
      e('edit-room-active').checked=!!r?.isActive;
   });
 }
 async function init(){
  try{
    const response=await fetch(cfg.apiUrl,{credentials:'same-origin',headers:{Accept:'application/json'}});
    if(!response.ok)throw new Error('Bina kayıtları yüklenemedi.');
    data=await response.json();
    options(bsel,data,'Bina seçiniz...');bsel.addEventListener('change',syncFloors);
    const editURL=cfg.apiUrl.replace(/\/Hierarchy(?:\?.*)?$/,'/ManagementHierarchy');
    const allResponse=await fetch(editURL,{credentials:'same-origin'});
    all=allResponse.ok?await allResponse.json():data;
    list.textContent=all.length?all.map(b=>`${b.name}: ${(b.floors||[]).length} kat, ${(b.floors||[]).reduce((n,f)=>n+(f.rooms||[]).length,0)} oda`).join(' • '):'Henüz bina tanımlanmadı.';
    buildEdit();
    renderMap();
    message.textContent='Binayı yerleştirmek için haritaya tıklayın. Kayıtlı binayı seçmek için işaretine tıklayın.';
  }catch(err){message.textContent=err.message;renderMap();}
 }
 init();
})();

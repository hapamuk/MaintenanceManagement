/* Kocaeli ilçe/mahalle seçimleri: TurkiyeAPI (v1), harita odağı: OSM Nominatim.
   Veri her kullanıcı seçiminde otomatik gelir; bina kayıtları sizin SQL Server'ınızda kalır.
   Nominatim yalnızca açık kullanıcı işlemlerinde ve önbellekli olarak çağrılır. */
(() => {
'use strict';
const API='https://api.turkiyeapi.dev/v1';
const districtCache={}; const neighborhoodCache=new Map(); const geoCache=new Map();
const AREA='Kocaeli'; const center=[40.7654,29.9408];
let geoLast=0; let geoBusy=Promise.resolve();
const cmp=s=>String(s||'').trim().toLocaleLowerCase('tr-TR');
function setOptions(el,entries,label,selected='') {
 if(!el)return;
 el.replaceChildren(new Option(label,''));
 entries.forEach(x=>{const o=new Option(x.name,x.name);if(x.id)o.dataset.id=String(x.id);el.add(o);});
 el.value=entries.some(x=>cmp(x.name)===cmp(selected))?entries.find(x=>cmp(x.name)===cmp(selected)).name:'';
 el.disabled=entries.length===0;
}
async function json(url) {
 const res=await fetch(url,{headers:{Accept:'application/json'}});
 if(!res.ok)throw new Error('Adres hizmeti HTTP '+res.status+' yanıtı verdi.');
 const payload=await res.json();
 if(payload.status && payload.status!=='OK')throw new Error('Adres servisi yanıtı geçersiz.');
 return payload.data;
}
async function districts(){
 if(districtCache.rows)return districtCache.rows;
 const rows=await json(API+'/districts?provinceId=41&limit=100');
 if(!Array.isArray(rows))throw new Error('İlçeler okunamadı.');
 districtCache.rows=rows.filter(x=>Number(x.provinceId)===41)
  .map(x=>({id:x.id,name:x.name})).sort((a,b)=>a.name.localeCompare(b.name,'tr'));
 return districtCache.rows;
}
async function neighborhoods(districtId){
 if(!districtId)return [];
 if(neighborhoodCache.has(districtId))return neighborhoodCache.get(districtId);
 const item=await json(API+'/districts/'+encodeURIComponent(districtId));
 const rows=(item?.neighborhoods||[]).map(x=>({id:x.id,name:x.name})).sort((a,b)=>a.name.localeCompare(b.name,'tr'));
 neighborhoodCache.set(districtId,rows);return rows;
}
function setProvince(sel,selected=''){setOptions(sel,[{name:AREA}],'Tüm iller',selected);}
async function loadDistrictSelect(select,selected=''){
 try {setOptions(select,[],'İlçeler yükleniyor...');const rows=await districts();setOptions(select,rows,'Tüm ilçeler',selected);return rows;}
 catch(error){setOptions(select,[],'İlçeler yüklenemedi');throw error;}
}
async function loadNeighborhoodSelect(select,districtSelect,selected=''){
 const id=districtSelect?.selectedOptions?.[0]?.dataset.id;
 if(!id){setOptions(select,[],'Önce ilçe seçiniz');return [];}
 try{setOptions(select,[],'Mahalleler yükleniyor...');const rows=await neighborhoods(id);
  setOptions(select,rows,'Tüm mahalleler',selected);return rows;}
 catch(error){setOptions(select,[],'Mahalleler yüklenemedi');throw error;}
}
// Yalnızca kullanıcının bilinçli seçiminden sonra arama yapılır; klavye başına API isteği yoktur.
function nominatim(url){
 if(geoCache.has(url))return Promise.resolve(geoCache.get(url));
 geoBusy=geoBusy.catch(()=>{}).then(async()=>{
  await new Promise(r=>setTimeout(r,Math.max(0,1100-(Date.now()-geoLast))));geoLast=Date.now();
  const response=await fetch(url,{headers:{Accept:'application/json'}});
  if(!response.ok)throw new Error('Harita konum servisi şu anda yanıt vermiyor.');
  const result=await response.json();geoCache.set(url,result);return result;
 });return geoBusy;
}
async function focus(map,province,district,neighborhood){
 if(!map)return;
 const address=[neighborhood,district,province||AREA,'Türkiye'].filter(Boolean).join(', ');
 const url='https://nominatim.openstreetmap.org/search?format=jsonv2&limit=1&countrycodes=tr&q='+encodeURIComponent(address);
 const result=await nominatim(url);const hit=result?.[0];
 if(!hit)return false;
 const box=hit.boundingbox?.map(Number);
 if(box?.length===4&&box.every(Number.isFinite))map.fitBounds([[box[0],box[2]],[box[1],box[3]]],{padding:[30,30],maxZoom:16});
 else map.setView([Number(hit.lat),Number(hit.lon)],neighborhood?16:district?12:10);
 return true;
}
async function reverse(lat,lng){
 const url='https://nominatim.openstreetmap.org/reverse?format=jsonv2&zoom=18&addressdetails=1&lat='
 +encodeURIComponent(Number(lat).toFixed(6))+'&lon='+encodeURIComponent(Number(lng).toFixed(6));
 const result=await nominatim(url);const a=result?.address||{};
 const province=a.province||a.state||'';
 const district=a.city_district||a.county||a.town||a.municipality||'';
 const neighborhood=a.neighbourhood||a.suburb||a.quarter||a.residential||'';
 return {province,district,neighborhood,displayName:result?.display_name||''};
}
window.AssetRegions={AREA,center,cmp,setOptions,setProvince,loadDistrictSelect,loadNeighborhoodSelect,focus,reverse};
})();

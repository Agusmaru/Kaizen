(()=>{
 const menu=document.getElementById('mainNav');
 if(menu){
  menu.addEventListener('click',event=>{
   if(!event.target.closest('.nav-link')||window.innerWidth>=992)return;
   bootstrap.Collapse.getOrCreateInstance(menu,{toggle:false}).hide();
  });
 }
 document.addEventListener('submit',event=>{
  setTimeout(()=>{
   if(!event.defaultPrevented)return;
   event.target.querySelectorAll('button:disabled').forEach(button=>button.disabled=false);
   event.target.querySelectorAll('.spinner-border').forEach(spinner=>spinner.classList.add('d-none'));
  },0);
 },true);
})();

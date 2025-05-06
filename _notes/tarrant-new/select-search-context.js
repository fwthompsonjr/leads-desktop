let search_type = '{0}'; // default to case records search
let location_name = '{1}'
const linkitems = [
    'Case Records Search'.toLowerCase(),
    'Class C Misdemeanors Only'.toLowerCase()
]
// Create a new 'change' event, set bubbles to true if you want the event to propagate up the DOM tree
const change_event = new Event('change', { bubbles: true }); 

let cbo = document.getElementById('sbxControlID2');
let opts = Array.prototype.slice.call(cbo.getElementsByTagName('option'), 0);
let links = Array.prototype.slice.call(document.getElementsByTagName('a'), 0);
links = links.filter(n => { return n.classList.contains('ssSearchHyperlink') });
links = links.filter(n => {
    let txt = String(n.innerText).trim().toLowerCase();
    for (let i of linkitems) {
        if (linkitems[i] == txt) { return true; }
    }
    return false;
});

if (isNaN(search_type)) return;
let searchId = parseInt(search_type);
if (searchId < 0 || links.length > searchId) return;
if (searchId == 1 && location_name.indexOf('JP') < 0) {
    location_name = 'All JP Courts'
}

let optId = opts.findIndex(x => x.innerText == location_name);
if (null == optId || optId < 0) { return; }
let lnk = links.find(x => x.innerText.trim().toLowerCase() == linkitems[searchId]);
if (lnk == null) return;

cbo.selectedIndex = optId;
cbo.dispatchEvent(change_event);
lnk.click();
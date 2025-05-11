let date0 = '{0}';
let date1 = '{1}';
let tboxes = [
    document.getElementById('DateFiledOnAfter'),
    document.getElementById('DateFiledOnBefore')
]
// Create a new 'change' event, set bubbles to true if you want the event to propagate up the DOM tree
const change_event = new Event('change', { bubbles: true });
const click_event = new Event('click', { bubbles: true });
const date_filed = 'Date Filed'
let cbo = document.getElementById('SearchBy');
let opts = Array.prototype.slice.call(cbo.getElementsByTagName('option'), 0);
let radio_buttons = Array.prototype.slice.call(document.getElementsByTagName('input'), 0);
radio_buttons = radio_buttons.filter(x => x.getAttribute('name') == 'CaseStatusType')
let open_option = radio_buttons.find(x => x.id == 'OpenOption');
let dateIndex = opts.findIndex(x => x.innerText == date_filed);

open_option.setAttribute('checked', 'checked');
cbo.selectedIndex = dateIndex;
cbo.dispatchEvent(change_event);

tboxes[0].value = date0
tboxes[1].value = date1

let bttn = document.getElementById('SearchSubmit');
bttn.click();
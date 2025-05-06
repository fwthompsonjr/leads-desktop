let human_check_found = false;
if (null != document.getElementById('captcha-container')) { human_check_found = true; }
if (!human_check_found) {
    let body = document.getElementsByTagName('body')[0];
    let txt = body.innerText;
    human_check_found = txt.indexOf('confirm you are human') > 0;
}
var rsp = {
    hasCaptcha = human_check_found
}
return JSON.stringify(rsp);
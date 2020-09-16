
function getPageDetail(asObject = false)
{
	var data = [];
	var rcount = $("#itemPlaceholderContainer tr.even").length;
	var rr = 0;
	$("#itemPlaceholderContainer tr.even")
		.each(function() { 			
			var $this = $(this);
			var statement = ("Reading : /0/ of /1/")
				.replace("/0/", rr++)
				.replace("/1/", rcount);
			overlay(statement);
			data.push( getRowDetail( $this ) );
		});
	removeOverlay();
	if (!asObject) {
		return JSON.stringify(data);
    }
	return data;
}

function getRowDetail($row)
{
	var caseData = {};
	for(var i = 1; i <= 8; i++)
	{
		var $cell = $($row.find("td")[i]);
		switch (i)
		{
			case 1:
				caseData.Case = $cell.find("a.doclinks").text();
				break;
			case 2:
				caseData.Court = $cell.text().trim();
				break;
			case 3:
				caseData.FileDate = $cell.text().trim();
				break;
			case 4:
				caseData.Status = $cell.text().trim();
				break;
			case 5:
				caseData.TypeDesc = $cell.text().trim();
				break;
			case 6:
				caseData.Subtype = $cell.text().trim();
				break;
			case 7:
				caseData.Style = $cell.text().trim();
				break;
			case 8:
				var link = $cell.find("a").attr("onclick");
				var n = link.indexOf("x-") + 1;
				link = link.substr(n);
				n = link.indexOf(".");
				link = link.substr(1,n - 1);
				$cell.find("a").click();
				caseData.CaseDataAddresses = gatAddresses( link );
				$cell.find("a").click();
				break;
		}
	}
	return caseData;
}

function gatAddresses( search ) {
	var data = [];
	$( "div[id*='" + search + "']" )
		.find("table[rules='rows'] tr[align='center']")
		.each(function() {
			$this = $(this);
			var obj = {};
			obj["Case"] = htmlDecode($($this.find("td")[0]).text().trim());
			obj["Role"] = htmlDecode($($this.find("td")[1]).text().trim());
			obj["Party"] = htmlDecode($($this.find("td")[2]).html().trim());
			obj["Attorney"] = htmlDecode($($this.find("td")[3]).html().trim());
			data.push(obj);
		});
	return data;
	
}

function htmlDecode(input){
  var e = document.createElement('textarea');
  e.innerHTML = input;
  // handle case of empty input
  var cleaned = e.childNodes.length === 0 ? "" : e.childNodes[0].nodeValue;
  return cleaned.replace(/<br\s*\/?>/mg," | ");
}

function displayOverlay(text) {
	$("<table id='overlay'><tbody><tr><td id='overlayText'>" + text + "</td></tr></tbody></table>").css({
		"position": "fixed",
		"top": 0,
		"left": 0,
		"width": "100%",
		"height": "100%",
		"background-color": "rgba(0,0,0,.5)",
		"z-index": 10000,
		"vertical-align": "middle",
		"text-align": "center",
		"color": "#fff",
		"font-size": "30px",
		"font-weight": "bold",
		"cursor": "wait"
	}).appendTo("body");
}

function removeOverlay() {
	$("#overlay").remove();
}

function overlay(text) {
	if ($("#overlay").length == 0) {
		displayOverlay(text);
		return;
	}
	$("#overlayText").text(text);
}
getPageDetail();

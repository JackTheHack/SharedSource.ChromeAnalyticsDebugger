var cookieName = 'Sitecore.Analytics.ChromeDebugger.SecretCookie';

chrome.webRequest.onBeforeSendHeaders.addListener(function(details) {
	if(localStorage.secretKey)
	{
		var blockingResponse = {};
		details.requestHeaders.push({ name: cookieName, value:  localStorage.secretKey});
		blockingResponse.requestHeaders = details.requestHeaders;
		return blockingResponse;
	}		
	
}, {urls: ['<all_urls>']}, ['requestHeaders', 'blocking']);
		
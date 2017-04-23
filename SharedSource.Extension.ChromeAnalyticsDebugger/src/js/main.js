localStorage.domLoaded = false;
localStorage.hasCookie = false;
var cookieName = 'Sitecore.Analytics.ChromeDebugger.SecretCookie';
localStorage.secretKey = undefined;

function init(){	

	$("#btnEnable").on("click", function(){
		$("#enabledDebugging").removeClass("hidden");
		$("#noDebug").addClass("hidden");
		
		localStorage.secretKey = 'Enabled '+ $("#secretKey").val();
		chrome.devtools.inspectedWindow.eval("window.location.reload();");
	});
	
	$("#btnDisable").on("click", function(){
		$("#enabledDebugging").addClass("hidden");
		$("#noDebug").removeClass("hidden");
		
		localStorage.secretKey = undefined;
	});

	$('#tabs a').on("click", function (e) {				
      $("#tabs li").removeClass("active");
	  $(this).parent("li").addClass("active");
	  $(".tab-content .tab-pane").removeClass("active");
	  $($(this).attr("href")).addClass('active');	  
	});	

	domLoaded = true;
}

function setPageProfileValues(analyticsData)
{
	$("#pageValues").html('');
	
	
	if(analyticsData.ContactIdentifier && analyticsData.ContactIdentifier.length)
	{
		$("#contactIdentifierBlock").removeClass("hidden");
		$("#contactIdentifier").html(analyticsData.ContactIdentifier);
	}else{
		$("#contactIdentifierBlock").addClass("hidden");
	}
	
	
					
					for(profileIndex in analyticsData.ProfileValues)
					{
						var profile = analyticsData.ProfileValues[profileIndex];
						var profileSection = $("<div class='profile-section col-sm-6 col-xs-12'/>");
						profileSection.append($('<label>'+profile.ProfileName+"</label>"));
						profileSection.append($('<div class="profile-identifier"/>').html(profile.ProfileId));
						var profileValueSection = $("<div class='profile-values-section'/>");						
						for(profileValueIndex in profile.Values)
						{
							var profileValue = profile.Values[profileValueIndex];
							profileValueSection.append($("<div class='profile-value'/>").html(profileValue.Key + ":" + profileValue.Value));
						}
						profileSection.append(profileValueSection);
						
						$("#pageValues").append(profileSection);
					}					
}

function setMatchedProfileValues(analyticsData){
	$("#matchedProfiles").html('');
					
					for(profileIndex in analyticsData.MatchedProfiles)
					{
						var profile = analyticsData.MatchedProfiles[profileIndex];
						var profileSection = $("<div class='profile-section col-sm-6 col-xs-12'/>");
						profileSection.append($('<label>'+profile.ProfileName+"</label>"));
						profileSection.append($('<div class="total-values"/>').html('Total:'+ profile.Total));
						profileSection.append($('<div class="matched-pattern"/>').html('Pattern:'+profile.PatternName));
						profileSection.append($('<div class="matched-pattern-id"/>').html(profile.PatternId));
						var profileValueSection = $("<div class='profile-values-section'/>");						
						for(profileValueIndex in profile.Values)
						{
							var profileValue = profile.Values[profileValueIndex];
							profileValueSection.append($("<div class='profile-value'/>").html(profileValue.Key + ":" + profileValue.Value));
						}
						profileSection.append(profileValueSection);
						
						$("#matchedProfiles").append(profileSection);
					}
}

function setGeolocationValues(analyticsData)
{
	$("#geolocationBlock").html('');
	
	if(analyticsData.GeoDetails)
	{
		var geoData = analyticsData.GeoDetails;		
		$("#geolocationBlock").append($("<div>Country: "+ geoData.Country + "</div>"));
		$("#geolocationBlock").append($("<div>City: "+ geoData.City + "</div>"));		
		$("#geolocationBlock").append($("<div>PostalCode: "+ geoData.PostalCode + "</div>"));
		$("#geolocationBlock").append($("<div>Latitude: "+ geoData.Latitude + "</div>"));
		$("#geolocationBlock").append($("<div>Longitude: "+ geoData.Longitude + "</div>"));		
		$("#geolocationBlock").append($("<div>Region: "+ geoData.Region + "</div>"));
		$("#geolocationBlock").append($("<div>AreaCode: "+ geoData.AreaCode + "</div>"));		
	}else{
		$("#geolocationBlock").html("Sorry, no geolocation data available");
	}
}

document.addEventListener('DOMContentLoaded', init);

if(chrome.devtools)
{
		chrome.devtools.network.onRequestFinished.addListener(function(request) {
			
			chrome.devtools.inspectedWindow.eval("window.location.href", function(result){
				windowHost = result;			
						
				if(request.request.url !== windowHost)
				{
					return;
				}
			
				headers = request.response.headers;
				var analyticsDataHeader = headers.find(function(x) { return x.name.toLowerCase() === 'sitecore.analytics.data' });
				var analyticsDataError = headers.find(function(x) { return x.name.toLowerCase() === 'sitecore.analytics.error' });
		  
				if(!analyticsDataHeader || (analyticsDataError && analyticsDataError.value === "InvalidKey"))
				{
					$("#enabledDebugging").addClass("hidden");
					$("#noDebug").removeClass("hidden");
				}else if(localStorage.secretKey && localStorage.secretKey.length){
					$("#enabledDebugging").removeClass("hidden");
					$("#noDebug").addClass("hidden");
				}
								
				if(analyticsDataHeader)
				{					
					var analyticsData = JSON.parse(analyticsDataHeader.value);					
												
					setPageProfileValues(analyticsData);
					setMatchedProfileValues(analyticsData);
					setGeolocationValues(analyticsData);

				}
		});	  
		
	});
		
}
function getRandomString(length=25) {
    var result           = '';
    var characters       = 'abcdefghijklmnopqrstuvwxyz0123456789';
    var charactersLength = characters.length;
    for ( var i = 0; i < length; i++ ) {
      result += characters.charAt(Math.floor(Math.random() * charactersLength));
   }
   return result;
}
function getEventId() {
    return Math.floor(new Date().getTime() / 1000) + '-' + getRandomString(25);
}


function setCookies(cname, cvalue, exdays) {
  const d = new Date();
  d.setTime(d.getTime() + (exdays*24*60*60*1000));
  let expires = "expires="+ d.toUTCString();
  document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
}
function getCookie(cname) {
  let name = cname + "=";
  let decodedCookie = decodeURIComponent(document.cookie);
  let ca = decodedCookie.split(';');
  for(let i = 0; i <ca.length; i++) {
    let c = ca[i];
    while (c.charAt(0) == ' ') {
      c = c.substring(1);
    }
    if (c.indexOf(name) == 0) {
      return c.substring(name.length, c.length);
    }
  }
  return "";
}

if( getCookie('userId').length < 1 ) {
    setCookies('userId',getEventId(),365);
}

// Google Tag Manager
if( typeof dataLayer === 'undefined' ) {
    let eventId =  getEventId();
    dataLayer = [{'eventId': eventId}];
    $.post( "https://specflow.org/wp-content/plugins/fb-sst/t.php", {
        'eventId':      eventId,
        'sourceUrl':    window.location.origin + window.location.pathname,
        'event_name':   'PageView',
        'external_id':  getCookie('userId'),
        'fbc':          getCookie('_fbc'),
        'fbp':          getCookie('_fbp'),
        'analytics_storage':getCookie('v_c_analytics_storage'),
    } );
} 
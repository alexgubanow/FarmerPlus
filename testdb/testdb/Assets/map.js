var map, light, old_val, selid, curre, currbaselayer;
var areas = new L.featureGroup();
function init() {
    var HYBRID = L.tileLayer('https://{s}.google.com/vt/lyrs=s,h&x={x}&y={y}&z={z}', {
        maxZoom: 26,
        subdomains: ['mt0', 'mt1', 'mt2', 'mt3']
    });
    var Google = L.tileLayer('https://{s}.google.com/vt/lyrs=m&x={x}&y={y}&z={z}', {
        maxZoom: 26,
        subdomains: ['mt0', 'mt1', 'mt2', 'mt3']
    });
    map = L.map('map', {
        //fadeAnimation: false,
        editable: true,
        center: [50.0031533, 36.3025844],
        zoom: 16,
        layers: [HYBRID, areas]
    });
    var baseLayers = {
        "Спутник": HYBRID,
        "Google": Google
    };
    var overlays = {
        "Паи": areas
    };
    L.control.layers(baseLayers, overlays).addTo(map);
    map.on('baselayerchange', mapbaselayerchange);
    L.control.scale({ imperial: false }).addTo(map);
    //HYBRID.on("load", function () { myGlobalObject.loadfin() });
    //var lat = (((sessionStorage.getItem('mapcoords')).replace("LatLng(", "")).replace(")", "")).split(',')[0];
    //var lng = (((sessionStorage.getItem('mapcoords')).replace("LatLng(", "")).replace(")", "")).split(',')[1];
    //map.setView([parseFloat(lat), parseFloat(lng)], parseFloat(sessionStorage.getItem('mapzoom')));
    try {
        
        var lat = (((localStorage.getItem('mapcoords')).replace("LatLng(", "")).replace(")", "")).split(',')[0];
        var lng = (((localStorage.getItem('mapcoords')).replace("LatLng(", "")).replace(")", "")).split(',')[1];
        map.setView([parseFloat(lat), parseFloat(lng)], parseFloat(localStorage.getItem('mapzoom')));

    } catch (err) {
    }
}
function savemapstate()
{
    //sessionStorage.setItem('mapcoords', map.getCenter());
    //sessionStorage.setItem('mapzoom', map.getZoom());
    localStorage.setItem('mapcoords', map.getCenter());
    localStorage.setItem('mapzoom', map.getZoom());
}

function lights(json, pan) {
    if (map.hasLayer(light)) { map.removeLayer(light) }
    var coords = JSON.parse(json);
    try
    {
        if (currbaselayer.name == "Спутник") {
            light = L.polyline(coords, { color: 'Yellow', weight: '2' }).addTo(map);
        }
        else {
            light = L.polyline(coords, { color: 'red', weight: '2' }).addTo(map);
        }
    }
    catch(ex)
    { light = L.polyline(coords, { color: 'Yellow', weight: '2' }).addTo(map); }

    if (pan) {
        map.fitBounds(light.getBounds());
    }
}
function mapbaselayerchange(e) {
    currbaselayer = e;
    if (map.hasLayer(light)) {
        var coords = light.getLatLngs();
        map.removeLayer(light);
        if (e.name == "Спутник") {
            light = L.polyline(coords, { color: 'Yellow', weight: '2' }).addTo(map);
        }
        else {
            light = L.polyline(coords, { color: 'red', weight: '2' }).addTo(map);
        }
    }
}
function loadarea(str, descript) {
    var lay = L.geoJson(JSON.parse(str), {
        style: function (feature) {            
            return { color: feature.properties.color, weight: feature.properties.weight };
        }
    }).bindPopup(descript);
    lay.on('click', onClick);
    areas.addLayer(lay);
}
function updatefdb() {
    var prevpopup = document.getElementsByClassName("leaflet-popup-content")[0].innerHTML;
    var tempDiv = document.createElement('div');
    tempDiv.innerHTML = prevpopup;
    tempDiv.getElementsByTagName("input")[0].defaultValue = document.getElementById("area_input").value;
    curre.layer.bindPopup(tempDiv.innerHTML);
    myGlobalObject.update([document.getElementById("index").innerText, document.getElementById("area_input").value]);
    //document.getElementById("savebtn").style.visibility = "hidden";
}
//function enblbutton() {
//    if (old_val != document.getElementById("area_input").value) {
//        document.getElementById("savebtn").style.visibility = "visible";
//        old_val = document.getElementById("area_input").value;
//    }
//    else {
//        document.getElementById("savebtn").style.visibility = "hidden";
//    }
//}
function pan_to(json) {
    if (map.hasLayer(light)) { map.removeLayer(light) }
    var coords = JSON.parse(json);
    coords.push(coords[0]);
    try {
        if (currbaselayer.name == "Спутник") {
            light = L.polyline(coords, { color: 'Yellow', weight: '3' }).addTo(map);
        }
        else {
            light = L.polyline(coords, { color: 'red', weight: '3' }).addTo(map);
        }
    }
    catch (ex)
    { light = L.polyline(coords, { color: 'Yellow', weight: '3' }).addTo(map); }
    map.addLayer(light);
    map.setView(coords[0], 15);
}
function onClick(e) {
    curre = e;
    e.layer.openPopup(e.latlng);
    old_val = document.getElementById("area_input").value;
}
function removeLayer() {
    if (map.hasLayer(light)) { map.removeLayer(light) }
}
var map, light, old_val, selid, curre, currbaselayer, rect, kmllay, okbutt, cancelbutt;
var areas = new L.featureGroup();
var kmllay = new L.featureGroup();
function init() {
    var HYBRID = L.tileLayer('https://{s}.google.com/vt/lyrs=s,h&x={x}&y={y}&z={z}', {
        maxZoom: 18,
        subdomains: ['mt0', 'mt1', 'mt2', 'mt3']
    });
    var Google = L.tileLayer('https://{s}.google.com/vt/lyrs=m&x={x}&y={y}&z={z}', {
        maxZoom: 18,
        subdomains: ['mt0', 'mt1', 'mt2', 'mt3']
    });
    map = L.map('map', {
        zoomAnimation: false,
        //fadeAnimation: false,
        editable: true,
        center: [48.5, 29.91],
        zoom: 6,
        layers: [HYBRID, areas, kmllay]
    });
    var baseLayers = {
        "Спутник": HYBRID,
        "Google": Google
    };
    var overlays = {
        "Паи": areas,
        "KML": kmllay
    };
    L.control.layers(baseLayers, overlays).addTo(map);
    map.on('baselayerchange', mapbaselayerchange);
    L.control.scale({ imperial: false }).addTo(map);
    try {
        
        var lat = (((localStorage.getItem('mapcoords')).replace("LatLng(", "")).replace(")", "")).split(',')[0];
        var lng = (((localStorage.getItem('mapcoords')).replace("LatLng(", "")).replace(")", "")).split(',')[1];
        map.setView([parseFloat(lat), parseFloat(lng)], parseFloat(localStorage.getItem('mapzoom')));

    } catch (err) {
    }
}
function savemapstate()
{
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
function loadkmlf(str, descript) {
    var lay = L.geoJson(JSON.parse(str), {
        style: function (feature) {
            return { color: "red" };
        }
    }).bindPopup("<div>" + descript + "</div><div>" + (geometry(JSON.parse(str)) / 10000).toFixed(4) + "</div>");
    lay.on('click', onClick);
    kmllay.addLayer(lay);
    map.fitBounds(lay.getBounds());
}
function loadarea(str, descript) {
    var jsonlay = JSON.parse(str);
    var lay = L.polygon(jsonlay.geometry.coordinates, { color: jsonlay.properties.color, weight: jsonlay.properties.weight }).bindPopup(descript);
    lay.on('click', onClick);
    areas.addLayer(lay);
}
function updatefdb() {
    var prevpopup = document.getElementsByClassName("leaflet-popup-content")[0].innerHTML;
    var tempDiv = document.createElement('div');
    tempDiv.innerHTML = prevpopup;
    tempDiv.getElementsByTagName("input")[0].defaultValue = document.getElementById("area_input").value;
    curre.bindPopup(tempDiv.innerHTML);
    myGlobalObject.update([document.getElementById("index").innerText, document.getElementById("area_input").value]);
}
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
function returnvalue()
{
    okbutt.removeFrom(map);
    cancelbutt.removeFrom(map);
    rect.disableEdit();
    map.removeLayer(rect);
    myGlobalObject.returnvalue(true);
}
function enselrect() {
    okbutt = L.easyButton('<span>&check;</span>', returnvalue).addTo(map);
    cancelbutt = L.easyButton('<span>&cross;</span>', disselrect).addTo(map);
    rect = L.rectangle(map.getBounds()).setStyle({ color: 'black' }).addTo(map);
    rect.enableEdit();
    map.setZoom(map.getZoom() -1, false);
}
function disselrect() {
    okbutt.removeFrom(map);
    cancelbutt.removeFrom(map);
    rect.disableEdit();
    map.removeLayer(rect);
    myGlobalObject.returnvalue(false);
}
function pan_to_sil(json) {
    var coords = JSON.parse(json);
    map.setView(JSON.parse(json), 13);
}
function pan_to_ray(json) {
    var coords = JSON.parse(json);
    map.setView(JSON.parse(json), 10);
}
function pan_to_obl(json) {
    var coords = JSON.parse(json);
    map.setView(JSON.parse(json), 8);
}
function onClick(e) {
    curre = map._layers[e.target._leaflet_id];
    //e.layer.openPopup(e.latlng);
    old_val = document.getElementById("area_input").value;
}
function removeLayer() {
    if (map.hasLayer(light)) { map.removeLayer(light) }
}
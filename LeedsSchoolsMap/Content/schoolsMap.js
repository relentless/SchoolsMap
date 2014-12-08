
function initialize() {
    var mapOptions = {
        center: new google.maps.LatLng(53.7996388, -1.5491220999999768),
        zoom: 12
    };
    var map = new google.maps.Map(document.getElementById("map-canvas"), mapOptions);

    $.ajax({
        url: document.URL + "schools",
        type: 'GET',
        contentType: "application/json",
        dataType: "json",
        success: function (data) {

            for (index = 0; index < data.length; ++index) {
                var school = data[index]
                var myLatlng = new google.maps.LatLng(school.Lat, school.Lon);
                var marker = new google.maps.Marker({
                    position: myLatlng,
                    map: map,
                    title: school.Name + "\nType: " + school.Type + "\nRank: " + school.Rank + "\nScore: " + school.Score,
                    icon: 'Content/' + school.Icon
                });
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert(errorThrown);
        }
    });

}

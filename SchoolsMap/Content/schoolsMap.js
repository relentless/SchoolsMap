function initialize() {
    var mapOptions = {
        center: new google.maps.LatLng(53.7996388, -1.5491220999999768),
        zoom: 14
    };
    var map = new google.maps.Map(document.getElementById("map-canvas"), mapOptions);

    $.ajax({
        url: "http://localhost:3579/schools",
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
                    title: school.Name + "\nRank " + school.Rank,
                    icon: 'Content/1stQuartile.png'
                });
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert(errorThrown);
        }
    });

}

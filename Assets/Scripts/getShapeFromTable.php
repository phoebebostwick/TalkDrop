<?php
    header("Access-Control-Allow-Credentials: true");
    header('Access-Control-Allow-Origin: *');
    header('Access-Control-Allow-Methods: POST, GET, OPTIONS');
    header('Access-Control-Allow-Headers: Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time');

    //server connection info
    $servername = "phoebebostwick.com";
    $server_username = "phoebebo_sproj";
    $server_password = "password123";
    $dbName = "phoebebo_thoughtsharing";

    //make connection
    $conn = new mysqli($servername, $server_username, $server_password, $dbName);
    
    if (!$conn) {
        die("Connection failed: " . mysqli_connect_error());
    }

    //Get info from table 
    $query = "SELECT * FROM `highscores` ORDER by `score` DESC LIMIT 10";
    $result = mysqli_query($conn, $query);

    //echo the list of results
    if (mysqli_num_rows($result) > 0) {
        while ($row = mysqli_fetch_assoc($result)) {
            echo '<li>';
            echo "name: " . $row['name']  . " score: " . $row['score'];
            echo '</li>';

        }
    }
?>
<?php 

    header("Access-Control-Allow-Credentials: true");
    header('Access-Control-Allow-Origin: *');
    header('Access-Control-Allow-Methods: POST, GET, OPTIONS');
    header('Access-Control-Allow-Headers: Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time');
    $servername = "phoebebostwick.com";
    $server_username =  "phoebebo_sproj";
    $server_password = "password123";
    $dbName = "phoebebo_thoughtsharing";

    // Strings must be escaped to prevent SQL injection attack. 
    $name = $_POST["namePost"];
    $hash = $_POST["hashPost"];
    $audio = $_POST["audioPost"];

    //make connection
    $conn = new mysqli($servername, $server_username, $server_password, $dbName);

    if (!$conn) {
        die("Connection failed: " . mysqli_connect_error());
        echo ("connection failed");
    } else {
        echo ("connection success");
    }

    $secretKey="sprojSecretKey";
    $real_hash = md5($name . $audio . $secretKey);

    if ($real_hash == $hash) {
        $sql = "INSERT INTO highscores (name, audiostring) VALUES ('".$name."','".$audio."')";
        $result = mysqli_query($conn ,$sql);
        echo $name . $audio;
    }

?>
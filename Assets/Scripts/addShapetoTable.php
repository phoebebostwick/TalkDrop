<?php 

    header("Access-Control-Allow-Credentials: true");
    header('Access-Control-Allow-Origin: *');
    header('Access-Control-Allow-Methods: POST, GET, OPTIONS');
    header('Access-Control-Allow-Headers: Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time');

    //server connection info
    $servername = "phoebebostwick.com";
    $server_username =  "phoebebo_sproj";
    $server_password = "password123";
    $dbName = "phoebebo_thoughtsharing";

    // Strings must be escaped to prevent SQL injection attack. 
    $name = $_POST["namePost"];
    $score = $_POST["scorePost"]; 
    $hash = $_POST["hashPost"];

    //make connection
    $conn = new mysqli($servername, $server_username, $server_password, $dbName);

    if (!$conn) {
        die("Connection failed: " . mysqli_connect_error());
        echo ("connection failed");
    } else {
        echo ("connection success");
    }

    //Method Authentication Code - reference HMAC, makes sure what we're sending is a legitimate message from a legitimate source, because that source has the key. Apparently this isn't very secure but I don't think we need to worry about it.
    $secretKey="sprojSecretKey";

    //combine key with data for the database. md5 is a function that takes the parameters and gives it a 32 digit hexadecimal ID
    $real_hash = md5($name . $score . $secretKey);

    //if the hashes are the same, the data is legitimate and can be added to the database table
    if ($real_hash == $hash) {
        $sql = "INSERT INTO highscores (name, score) VALUES ('".$name."','".$score."')";
        $result = mysqli_query($conn ,$sql);
        echo $name . $score;
    }

?>
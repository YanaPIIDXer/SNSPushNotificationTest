<?php
    $title = $_POST["title"];
    $message = $_POST["message"];
    $sound = $_POST["sound"];
    if (strlen($title) == 0 || strlen($message) == 0)
    {
        echo "Error.";
        return;
    }

    $api_endpoint = "https://6sf41e1u56.execute-api.ap-northeast-1.amazonaws.com/default/publish_push_notification";
    $send_data = array(
        "title" => $title,
        "message" => $message,
        "sound" => $sound
    );
    $send_json = json_encode($send_data);
    
    $context = array(
        "http" => array(
            "method" => "POST",
            "header" => "Content-Type: application/json; charset=UTF-8",
            "content" => $send_json
        )
    );

    $result = file_get_contents($api_endpoint, false, stream_context_create($context));
    header("Location: index.html");
?>
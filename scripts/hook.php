<?php
$secret = "secret";
$command = "sh up.sh";
$branch = "production";
$ranges = array("192.30.252", "185.199.108");

function validate() {
  global $secret, $command, $branch, $ranges;
  $remote = ip2long($_SERVER['REMOTE_ADDR']);
  $valid = false;
  foreach($ranges as $range) {
    if($remote <= ip2long($range.".255") && $remote >= ip2long($range.".1")) {
      $valid = true;
      break;
    }
  }
  if(!$valid) {
    throw new Exception("Access denied");
  }
  if(!isset($_SERVER['HTTP_X_HUB_SIGNATURE'])) {
    throw new Exception("No signature");
  }
  $data = file_get_contents("php://input");
  list($algo, $hash) = explode("=", $_SERVER["HTTP_X_HUB_SIGNATURE"], 2);
  $sig = hash_hmac("sha1", $data, $secret);
  if(!hash_equals($hash, $sig)) {
    throw new Exception("Not valid signature");
  }
  $payload = json_decode($data);
  if($payload->repository->url != "https://github.com/Ronnrein/ItsyBits") {
    throw new Exception("Wrong repository");
  }
  if($payload->ref != "refs/heads/".$branch) {
    throw new Exception("Wrong branch");
  }
}

try {
  validate();
  shell_exec($command);
  file_put_contents("log", date('Y-m-d H:i:s')." - Success\r\n", FILE_APPEND);
}
catch (Exception $e) {
  echo $e->getMessage();
  file_put_contents("log", date('Y-m-d H:i:s')." - ".$e->getMessage()."\r\n", FILE_APPEND);
}
?>

function upClick(textId) {
  var text = document.getElementById(textId);
  var value = text.value ? parseInt(text.value) : 0;
  value++;
  text.value = value.toString();
}
function downClick(textId) {
  var text = document.getElementById(textId);
  var value = text.value ? parseInt(text.value) : 0;
  if (value > 0) value--;
  text.value = value.toString();
}

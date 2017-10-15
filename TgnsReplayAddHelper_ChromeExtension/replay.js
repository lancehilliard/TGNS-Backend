setInterval(function() {
  var doneProgressBars = document.getElementsByClassName('progress-bar-text-done');
  for (var i = 0; i < doneProgressBars.length; i++) {
    var doneProgressBar = doneProgressBars[i];
    var doneProgressBarIsVisible = doneProgressBar.offsetParent !== null;
    if (doneProgressBarIsVisible) {
      var uploadItemAncestorElement = doneProgressBar.closest('.upload-item');
      if (uploadItemAncestorElement && !uploadItemAncestorElement.hasAttribute("data-replay-add-attempted")) {
        var videoSettingsTitleInputs = uploadItemAncestorElement.getElementsByClassName('video-settings-title');
        if (videoSettingsTitleInputs.length > 0) {
          var videoSettingsTitleInput = videoSettingsTitleInputs[0];
          var watchPageLinkDivs = uploadItemAncestorElement.getElementsByClassName('watch-page-link');
          if (watchPageLinkDivs.length > 0) {
            var watchPageLinkDiv = watchPageLinkDivs[0];
            var watchPageLinks = watchPageLinkDiv.getElementsByTagName('a');
            if (watchPageLinks.length > 0) {
              var watchPageLink = watchPageLinks[0];
              var watchPageLinkUrlParts = watchPageLink.href.split('/');
              if (watchPageLinkUrlParts.length > 0) {
                var videoIdentifier = watchPageLinkUrlParts[watchPageLinkUrlParts.length - 1];
                if (videoIdentifier.length === 11) {
                  var videoSettingsTitle = videoSettingsTitleInput.value;
                  var videoSettingsTitleParts = videoSettingsTitle.split(' ');
                  if (videoSettingsTitleParts.length > 1) {
                    var gameStartTimeSeconds = videoSettingsTitleParts[0];
                    var videoSettingsTitleSecondPart = videoSettingsTitleParts[1];
                    if ((Number(parseFloat(gameStartTimeSeconds)) == gameStartTimeSeconds) && videoSettingsTitleSecondPart === 'TGNS') {
                      chrome.runtime.sendMessage({
                        method: 'POST',
                        action: 'xhttp',
                        url: 'http://rr.tacticalgamer.com/Recordings/Add',
                        data: 'gameStartTimeSeconds=' + gameStartTimeSeconds + '&videoIdentifier=' + videoIdentifier + '&returnJson=true'
                      }, function(responseText) {
                        if (responseText.indexOf('We need your Steam ID') !== -1) {
                          responseText = 'Not logged into TGNS Portal!';
                        }
                        // console.log('gameStartTimeSeconds: ' + gameStartTimeSeconds);
                        // console.log('videoSettingsTitleSecondPart: ' + videoSettingsTitleSecondPart);
                        // console.log('videoIdentifier: ' + videoIdentifier);
                        // console.log('responseText: ' + responseText);
                        var replayAddResultDisplayContainer = document.createElement('div');
                        if (responseText === '') {
                          replayAddResultDisplayContainer.innerHTML = 'Replay Add Success!';
                          replayAddResultDisplayContainer.style.color = 'green';
                        } else {
                          replayAddResultDisplayContainer.innerHTML = 'Replay Add Error: ' + responseText;
                          replayAddResultDisplayContainer.style.color = 'red';
                        }
                        watchPageLinkDiv.appendChild(replayAddResultDisplayContainer);

                      });
                      uploadItemAncestorElement.setAttribute('data-replay-add-attempted', true);
                    }
                  }
                }
              }
            }
          }
        }
      }
    }
  }
}, 1000);
// ==UserScript==
// @name         TGNS Replay Add Helper
// @namespace    http://rr.tacticalgamer.com/Replay
// @version      0.2
// @description  Automatically add to TGNS Replay any TGNS Recording Helper videos you upload to YouTube
// @downloadURL  http://rr.tacticalgamer.com/assets/userscripts/TgnsReplayAddHelper.user.js
// @author       https://steamcommunity.com/id/wyzcrak
// @match        https://www.youtube.com/upload
// @grant        GM_xmlhttpRequest
// @connect      rr.tacticalgamer.com
// ==/UserScript==

(function () {
    'use strict';

    var getVisibleVideoActionButtonByClassNameAndText = function (uploadItemContainer, className, text) {
        var result;
        var publishButtons = uploadItemContainer.getElementsByClassName(className);
        if (publishButtons.length > 0) {
            var publishButton = publishButtons[0];
            var isVisible = publishButton.offsetWidth > 0 || publishButton.offsetHeight > 0;
            if (!publishButton.disabled && isVisible) {
                var publishButtonSpans = publishButton.getElementsByTagName('span');
                if (publishButtonSpans.length > 0) {
                    var publishButtonSpan = publishButtonSpans[0];
                    var publishButtonText = publishButtonSpan.innerHTML;
                    if (publishButtonText == text) {
                        result = publishButton;
                    }
                }
            }
        }
        return result;
    };

    var isPrivate = function (uploadItemContainer) {
        var result = false;
        var privacySelects = uploadItemContainer.getElementsByClassName('metadata-privacy-input');
        if (privacySelects.length > 0) {
            var privacySelect = privacySelects[0];
            var selectedValue = privacySelect.options[privacySelect.selectedIndex].value;
            result = selectedValue == 'private';
        }
        return result;
    };

    var showReplayResult = function (uploadItemContainer, watchPageLinkDiv, errorMessage) {
        var replayAddResultDisplayContainer = document.createElement('div');
        if (errorMessage) {
            replayAddResultDisplayContainer.innerHTML = 'Replay Add Error: ' + errorMessage;
            replayAddResultDisplayContainer.style.color = 'red';
            uploadItemContainer.style.borderColor = '#ebcccc';
        } else {
            replayAddResultDisplayContainer.innerHTML = 'Replay Add Success!';
            replayAddResultDisplayContainer.style.color = 'green';
            uploadItemContainer.style.borderColor = '#d0e9c6';
        }
        uploadItemContainer.style.borderWidth = 'thick';
        watchPageLinkDiv.appendChild(replayAddResultDisplayContainer);
        uploadItemContainer.classList.remove('accordion-collapsed');
        uploadItemContainer.classList.add('accordion-expanded');
    };

    setInterval(function () {


        var finishedUploadItems = document.getElementsByClassName('upload-item-finished');
        for (var i = 0; i < finishedUploadItems.length; i++) {
            var finishedUploadItem = finishedUploadItems[i];
            if (!finishedUploadItem.hasAttribute("data-replay-add-attempted")) {
                var videoSettingsTitleInputs = finishedUploadItem.getElementsByClassName('video-settings-title');
                if (videoSettingsTitleInputs.length > 0) {
                    var videoSettingsTitleInput = videoSettingsTitleInputs[0];
                    var watchPageLinkDivs = finishedUploadItem.getElementsByClassName('watch-page-link');
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
                                            var neededPublishButton = getVisibleVideoActionButtonByClassNameAndText(finishedUploadItem, 'save-changes-button', 'Publish');
                                            if (neededPublishButton) {
                                                var evt = document.createEvent('HTMLEvents');
                                                evt.initEvent('click', true, true);
                                                neededPublishButton.dispatchEvent(evt);
                                            } else {
                                                if (isPrivate(finishedUploadItem)) {
                                                    showReplayResult(finishedUploadItem, watchPageLinkDiv, 'Video is private. Upload as Public or Unlisted.');
                                                    finishedUploadItem.setAttribute('data-replay-add-attempted', true);
                                                } else {
                                                    var doneButton = getVisibleVideoActionButtonByClassNameAndText(finishedUploadItem, 'save-changes-button', 'Done');
                                                    var saveChangesButton = getVisibleVideoActionButtonByClassNameAndText(finishedUploadItem, 'save-changes-button', 'Save changes');
                                                    var returnToEditingButton = getVisibleVideoActionButtonByClassNameAndText(finishedUploadItem, 'return-to-editing-button', 'Return to editing');
                                                    var videoIsPublicallyAvailable = doneButton || saveChangesButton || returnToEditingButton;
                                                    if (videoIsPublicallyAvailable) {
                                                        GM_xmlhttpRequest({
                                                            method: "GET",
                                                            url: 'http://rr.tacticalgamer.com/Recordings/AddApi?gameStartTimeSeconds=' + gameStartTimeSeconds + '&videoIdentifier=' + videoIdentifier,
                                                            onload: function (response) {
                                                                var errorMessage;
                                                                if (response.responseText.indexOf('We need your Steam ID') !== -1) {
                                                                    errorMessage = 'Not logged into TGNS Portal!';
                                                                } else {
                                                                    var result = JSON.parse(response.responseText) || {};
                                                                    if (!result.success) {
                                                                        errorMessage = result.msg || 'Unknown error.';
                                                                    }
                                                                }
                                                                showReplayResult(finishedUploadItem, watchPageLinkDiv, errorMessage);
                                                            }
                                                        });
                                                        finishedUploadItem.setAttribute('data-replay-add-attempted', true);
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
            }
        }
    }, 2000);
})();
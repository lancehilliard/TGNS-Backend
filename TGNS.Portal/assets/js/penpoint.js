function penpointInitEdit(r) {
    var sketchpad;
    var needToNotifyAboutShareButton = true;

    function penpoint(imageUrlInput) {
        var imageUrl = r.GetImageBaseUrl + '?url=' + encodeURIComponent(imageUrlInput);
        $("<img />").attr("src", imageUrl).error(function () { alert('There was an error loading the following image:\n\n' + imageUrlInput); }).load(function () {

            $("#prepare").hide();

            var imageDimensions = {};
            imageDimensions.width = this.width;
            imageDimensions.height = this.height;

            sketchpad = Raphael.sketchpad("editor", {
                width: imageDimensions.width,
                height: imageDimensions.height,
                strokes: r.SketchJson,
                editing: true
            });
    
            function update() {
                var id = $("#id").val();
                var imageUrl = $("#imagePathInput").val();
                var sketchJson = $("#data").val();
                $.post(r.SaveUrl, { id: id, imageUrl: imageUrl, sketchJson: sketchJson }, function (data) {
                    $("#id").val(data.id);
                });
            }

            $("#shareButton").click(function () {
                var shareButton = $(this);
                shareButton.attr('disabled', 'disabled');
                var shareId = $("#id").val();
                if (shareId === '') {
                    var shareDiv = $('<div><p>This Penpoint is not shareable yet.</p><p>Draw some lines before sharing.</p></div>');
                    shareDiv.dialog({ width: 595, title: 'Share', modal: true });
                    shareButton.removeAttr('disabled');
                } else {
                    var backgroundImageCanvas = document.getElementById("backgroundImageCanvas");
                    backgroundImageCanvas.setAttribute('width', imageDimensions.width);
                    backgroundImageCanvas.setAttribute('height', imageDimensions.height);
                    var backgroundImageCanvasContext = backgroundImageCanvas.getContext("2d");
                    var taintProofImage = new Image();
                    taintProofImage.setAttribute('crossOrigin', 'Anonymous');
                    taintProofImage.onload = function () {
                        backgroundImageCanvasContext.drawImage(taintProofImage, 0, 0, imageDimensions.width, imageDimensions.height);

                        var sketchpadSvg = sketchpad.paper().toSVG();

                        //Use canvg to draw the SVG onto the empty canvas
                        var sketchpadCanvas = document.getElementById('sketchpadCanvas');
                        sketchpadCanvas.setAttribute('width', imageDimensions.width);
                        sketchpadCanvas.setAttribute('height', imageDimensions.height);
                        canvg(sketchpadCanvas, sketchpadSvg);

                        // setTimeout(function() {
                        //fetch the dataURL from the canvas and set it as src on the image
                        var sketchpadImage = document.getElementById('sketchpadImage');
                        sketchpadImage.setAttribute('width', imageDimensions.width);
                        sketchpadImage.setAttribute('height', imageDimensions.height);
                        sketchpadImage.onload = function () {
                            backgroundImageCanvasContext.drawImage(sketchpadImage, 0, 0, imageDimensions.width, imageDimensions.height);
                            var combinedImage = document.getElementById('combinedImage');
                            combinedImage.setAttribute('width', imageDimensions.width);
                            combinedImage.setAttribute('height', imageDimensions.height);
                            var combinedDataUrl = backgroundImageCanvas.toDataURL("image/png");
                            combinedImage.src = combinedDataUrl;

                            var imgurUrl;
                            $.ajax({
                                url: 'https://api.imgur.com/3/image',
                                type: 'post',
                                headers: {
                                    Authorization: 'Client-ID a8ed7d785042095'
                                },
                                data: {
                                    image: combinedDataUrl.split(',')[1]
                                },
                                dataType: 'json',
                                success: function (response) {
                                    if (response.success) {
                                        imgurUrl = response.data.link;
                                    }
                                },
                                complete: function () {
                                    var shareUrl = r.BaseUrl + shareId;
                                    var shareDiv = $('<div style="margin-left: 10px;"><div>View/Edit using Penpoint: <input type="text" value="' + shareUrl + '" class="shareUrlInput" style="margin: 0 0 4px 3px; width: 535px;" /></div><div>View without Penpoint: <input type="text" value="' + imgurUrl + '" class="shareUrlInput" style="margin: 0 0 4px 3px; width: 535px;" /></div></div>');
                                    shareDiv.dialog({ width: 595, title: 'Share', modal: true });
                                    shareDiv.find('input').blur();
                                    shareButton.removeAttr('disabled');
                                }
                            });

                        };
                        sketchpadImage.src = sketchpadCanvas.toDataURL("image/png");
                        // }, 100);
                    };
                    taintProofImage.src = imageUrl;
                }
            });

            sketchpad.change(function() {
                $("#undoButton").removeAttr('disabled');
                $("#redoButton").removeAttr('disabled');
                $("#cloneAdvisoryButton").removeAttr('disabled');
                $("#data").val(sketchpad.json());
                update();
                if (needToNotifyAboutShareButton) {
                    $("#shareButton").notify("Be sure to click 'Share' when you're done!", {className: 'info', autoHide: true});
        
                    needToNotifyAboutShareButton = false;
                }
            });
    
            var editorDiv = $("#editor");
            editorDiv.width(imageDimensions.width);
            editorDiv.height(imageDimensions.height);
            editorDiv.css("background-image", "url(" + imageUrl + ")");

            $("#edit").width(imageDimensions.width).show();
    
        });
    }
    
    $("#imagePathButton").click(function() {
        var imageUrl = $("#imagePathInput").val();
        penpoint(imageUrl);
    });
    
    if (r.EditDataId === '') {
        $("#prepare").show();
        $('#imagePathInput').focus().keypress(function keypressHandler(e)
        {
            if(e.which == 13) {
                $(this).blur();
                $('#imagePathButton').focus().click();
            }
        });
        var examplePenpointUrl = r.BaseUrl + 'b0COKPcpHkKFSqEco85cig';
        $("#examplePenpointLink").attr('href',examplePenpointUrl).text(examplePenpointUrl);
    }
    else {
        penpoint(r.EditDataImageUrl);
    }
    
    $("input[class='color']").change(function(){
        sketchpad.pen().color('#'+this.color);
    });
    
    $("#undoButton").click(function(){ sketchpad.undo();});
    $("#redoButton").click(function(){sketchpad.redo();});
    $("#clearButton").click(function(){sketchpad.clear();});
    $("#drawEraseButton").click(function(){
        var buttonText = this.value;
        if (buttonText === 'Draw') {
            this.value = 'Erase';
            sketchpad.editing("erase");
        }
        else {
            this.value = 'Draw';
            sketchpad.editing(true);
        }
    });
    $("#thinThickButton").click(function(){
        var buttonText = this.value;
        if (buttonText === 'Thin') {
            this.value = 'Thick';
            sketchpad.pen().width(15);
        }
        else {
            this.value = 'Thin';
            sketchpad.pen().width(5);
        }
    });
    
    $("#createNewButton").click(function () { location.href = r.BaseUrl; });
    
    $("#cloneAdvisoryButton").click(function(){
        var cloneButton = $('<input type="button" id="cloneButton" value="Got it. Clone this Penpoint!" />');
        cloneButton.click(function() {
            $.post(r.CloneUrl, { id: $("#id").val() }, function (data) {
                $("#cloneAdvisoryButton").attr('disabled','disabled');
                $("#id").val(data.id);
                $(".ui-dialog-content").dialog("close");
                var confirmationDiv = $('<div><p>Cloning successful!</p><p>Continue editing as you like.</p><p>Click the "Share" button anytime to get the new URL.<p>Great job! :)</p></div>');
                confirmationDiv.dialog({width:420,title: 'Success!',modal:true});
            });
        });
        var advisoryDiv = $('<div><p>Cloning uses this Penpoint\'s image and lines to create an identical Penpoint with its own URL.</p><p>The Penpoint now in the background will always be available at its URL, which you can get now by closing this dialog and clicking on the "Share" button.</p></div>');
        cloneButton.appendTo(advisoryDiv);
        advisoryDiv.dialog({width:420,title: 'Clone',modal:true});
    });
     
    if (r.EditDataId != '') {
        var advisoryDiv;
        if (r.UserIsOwner) {
            advisoryDiv = $('<div><p>This Penpoint is yours!</p><p>You\'re editing what other people might already be viewing.</p><p>Click "Clone" anytime to give further edits their own URL.</p><p>That\'s all! Enjoy! :)</p></div>');
            $("#cloneAdvisoryButton").removeAttr('disabled');
        } else {
            advisoryDiv = $('<div><p>This Penpoint was created by someone else.</p><p>It will be automatically cloned so that you may edit it. Cloning creates a new URL for your continued edits.</p><p>Make your edits (including any additional clones you\'d like to perform yourself), and be sure to click the "Share" button when you\'re done editing.</p><p>Enjoy! :)</p></div>');
        }
        advisoryDiv.dialog({width:420,title: 'A quick note before you begin!',modal:true});
    }
    
}
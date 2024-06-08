window.generateQRCode = function (qrCodeData, elementId) {
    new QRCode(document.getElementById(elementId), {
        text: qrCodeData,
        width: 256,
        height: 256
    });
}

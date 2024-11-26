function downloadFile(url) {
    const link = document.createElement('a');
    link.href = url;
    link.download = ''; // Este valor puede ser configurado desde el servidor.
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}
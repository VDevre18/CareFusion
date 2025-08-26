// Placeholder for wwwroot/js/interop.js
window.interop = {
    renderSampleChart: function (canvasId) {
        const canvas = document.getElementById(canvasId);
        if (!canvas) return;
        const ctx = canvas.getContext('2d');
        const data = [3, 5, 2, 8, 4, 6, 9];
        const max = Math.max(...data);
        const w = canvas.width, h = canvas.height;
        ctx.clearRect(0, 0, w, h);
        const barW = w / data.length - 10;
        data.forEach((v, i) => {
            const x = i * (barW + 10) + 5;
            const barH = (v / max) * (h - 20);
            ctx.fillRect(x, h - barH, barW, barH);
        });
    }
};

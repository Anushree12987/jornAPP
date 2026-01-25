window.renderMoodChart = function(labels, data) {
    const ctx = document.getElementById('moodChart').getContext('2d');
    if (window.moodChartInstance) window.moodChartInstance.destroy();
    window.moodChartInstance = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Mood Counts',
                data: data,
                backgroundColor: '#ffd700'
            }]
        },
        options: {
            responsive: true,
            plugins: { legend: { display: false } },
            scales: {
                y: { beginAtZero: true, ticks: { stepSize: 1 } }
            }
        }
    });
};

window.renderTrendChart = function(labels, data) {
    const ctx = document.getElementById('trendChart').getContext('2d');
    if (window.trendChartInstance) window.trendChartInstance.destroy();
    window.trendChartInstance = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'Mood Trends',
                data: data,
                fill: false,
                borderColor: '#ffd700',
                tension: 0.3,
                pointBackgroundColor: '#ffd700'
            }]
        },
        options: { responsive: true }
    });
};

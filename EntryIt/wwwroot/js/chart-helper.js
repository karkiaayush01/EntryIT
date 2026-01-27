// chart-helper.js
// Helper functions for rendering charts with Chart.js

let moodChartInstance = null;
let wordCountChartInstance = null;

window.renderMoodChart = function (labels, data) {
    const ctx = document.getElementById('moodChart');
    if (!ctx) return;

    // Get theme from MainLayout's .page element
    const isDarkTheme = document.querySelector('.page')?.classList.contains('theme-dark') ?? false;
    const textColor = isDarkTheme ? '#ccc' : '#333';
    const gridColor = isDarkTheme ? '#2b2b2b' : '#e5e5e5';

    // Always destroy and recreate for theme consistency
    if (moodChartInstance) {
        moodChartInstance.destroy();
        moodChartInstance = null;
    }

    moodChartInstance = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Mood Count',
                data: data,
                backgroundColor: [
                    'rgba(99, 102, 241, 0.8)',
                    'rgba(59, 130, 246, 0.8)',
                    'rgba(139, 92, 246, 0.8)',
                    'rgba(168, 85, 247, 0.8)',
                    'rgba(236, 72, 153, 0.8)',
                ],
                borderColor: [
                    'rgb(99, 102, 241)',
                    'rgb(59, 130, 246)',
                    'rgb(139, 92, 246)',
                    'rgb(168, 85, 247)',
                    'rgb(236, 72, 153)',
                ],
                borderWidth: 2,
                borderRadius: 8
            }]
        },
        options: {
            indexAxis: 'y',
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: false
                },
                tooltip: {
                    backgroundColor: isDarkTheme ? '#1a1a1a' : '#fff',
                    titleColor: textColor,
                    bodyColor: textColor,
                    borderColor: gridColor,
                    borderWidth: 1
                }
            },
            scales: {
                x: {
                    beginAtZero: true,
                    ticks: {
                        color: textColor,
                        stepSize: 1
                    },
                    grid: {
                        color: gridColor
                    }
                },
                y: {
                    ticks: {
                        color: textColor,
                        font: {
                            size: 13
                        }
                    },
                    grid: {
                        display: false
                    }
                }
            }
        }
    });
};

window.renderWordCountChart = function (labels, data) {
    const ctx = document.getElementById('wordCountChart');
    if (!ctx) return;

    // Get theme from MainLayout's .page element
    const isDarkTheme = document.querySelector('.page')?.classList.contains('theme-dark') ?? false;
    const textColor = isDarkTheme ? '#ccc' : '#333';
    const gridColor = isDarkTheme ? '#2b2b2b' : '#e5e5e5';

    // Always destroy and recreate for theme consistency
    if (wordCountChartInstance) {
        wordCountChartInstance.destroy();
        wordCountChartInstance = null;
    }

    wordCountChartInstance = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'Word Count',
                data: data,
                borderColor: 'rgb(99, 102, 241)',
                backgroundColor: 'rgba(99, 102, 241, 0.1)',
                borderWidth: 3,
                fill: true,
                tension: 0.4,
                pointRadius: 4,
                pointHoverRadius: 6,
                pointBackgroundColor: 'rgb(99, 102, 241)',
                pointBorderColor: '#fff',
                pointBorderWidth: 2
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: false
                },
                tooltip: {
                    backgroundColor: isDarkTheme ? '#1a1a1a' : '#fff',
                    titleColor: textColor,
                    bodyColor: textColor,
                    borderColor: gridColor,
                    borderWidth: 1,
                    callbacks: {
                        label: function (context) {
                            return context.parsed.y + ' words';
                        }
                    }
                }
            },
            scales: {
                x: {
                    ticks: {
                        color: textColor,
                        maxRotation: 45,
                        minRotation: 45
                    },
                    grid: {
                        color: gridColor
                    }
                },
                y: {
                    beginAtZero: true,
                    ticks: {
                        color: textColor
                    },
                    grid: {
                        color: gridColor
                    }
                }
            }
        }
    });
};

// Cleanup function
window.destroyCharts = function () {
    if (moodChartInstance) {
        moodChartInstance.destroy();
        moodChartInstance = null;
    }
    if (wordCountChartInstance) {
        wordCountChartInstance.destroy();
        wordCountChartInstance = null;
    }
};
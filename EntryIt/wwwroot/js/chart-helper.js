// chart-helper.js
// Helper functions for rendering charts with Chart.js

let moodChartInstance = null;
let wordCountChartInstance = null;
let tagChartInstance = null;

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

window.renderTagChart = function (labels, data) {
    const ctx = document.getElementById('tagChart');
    if (!ctx) return;

    // Get theme from MainLayout's .page element
    const isDarkTheme = document.querySelector('.page')?.classList.contains('theme-dark') ?? false;
    const textColor = isDarkTheme ? '#ccc' : '#333';

    // Always destroy and recreate for theme consistency
    if (tagChartInstance) {
        tagChartInstance.destroy();
        tagChartInstance = null;
    }

    // Generate vibrant colors for tags
    const colors = [
        'rgba(99, 102, 241, 0.8)',   // Indigo
        'rgba(59, 130, 246, 0.8)',   // Blue
        'rgba(16, 185, 129, 0.8)',   // Emerald
        'rgba(245, 158, 11, 0.8)',   // Amber
        'rgba(239, 68, 68, 0.8)',    // Red
        'rgba(168, 85, 247, 0.8)',   // Purple
        'rgba(236, 72, 153, 0.8)',   // Pink
        'rgba(20, 184, 166, 0.8)',   // Teal
        'rgba(251, 146, 60, 0.8)',   // Orange
        'rgba(34, 197, 94, 0.8)'     // Green
    ];

    const borderColors = [
        'rgb(99, 102, 241)',
        'rgb(59, 130, 246)',
        'rgb(16, 185, 129)',
        'rgb(245, 158, 11)',
        'rgb(239, 68, 68)',
        'rgb(168, 85, 247)',
        'rgb(236, 72, 153)',
        'rgb(20, 184, 166)',
        'rgb(251, 146, 60)',
        'rgb(34, 197, 94)'
    ];

    tagChartInstance = new Chart(ctx, {
        type: 'pie',
        data: {
            labels: labels,
            datasets: [{
                data: data,
                backgroundColor: colors.slice(0, labels.length),
                borderColor: borderColors.slice(0, labels.length),
                borderWidth: 2
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'bottom',
                    labels: {
                        color: textColor,
                        padding: 15,
                        font: {
                            size: 12
                        },
                        usePointStyle: true,
                        pointStyle: 'circle'
                    }
                },
                tooltip: {
                    backgroundColor: isDarkTheme ? '#1a1a1a' : '#fff',
                    titleColor: textColor,
                    bodyColor: textColor,
                    borderColor: isDarkTheme ? '#2b2b2b' : '#e5e5e5',
                    borderWidth: 1,
                    callbacks: {
                        label: function (context) {
                            const label = context.label || '';
                            const value = context.parsed || 0;
                            const total = context.dataset.data.reduce((a, b) => a + b, 0);
                            const percentage = ((value / total) * 100).toFixed(1);
                            return `${label}: ${value} (${percentage}%)`;
                        }
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
    if (tagChartInstance) {
        tagChartInstance.destroy();
        tagChartInstance = null;
    }
};
const cityInput = document.getElementById('cityInput');
const fetchDataButton = document.getElementById('fetchDataButton');
const weatherInfoEl = document.getElementById('weatherInfo');
const ipInfo = document.getElementById('ipInfo');
const newsList = document.getElementById('newsList');

fetchDataButton.addEventListener('click', async () => {
    const city = cityInput.value.trim();
    if (!city) {
        alert('Please enter a city name');
        return;
    }
    try {
        const response = await fetch(`Aggregation/GetAggregatedData?city=${city}`);

        if (response.ok) {
            const data = await response.json();
            updateWeatherInfo(data.weatherInfo);
            updateIpInfo(data.ipInfo);
            updateNewsCarousel(data.newsArticles);
        } else {
            alert('Error fetching data');
        }
    } catch (error) {
        alert('Error fetching data');
    }
});

function updateWeatherInfo(weatherInfo) {
    weatherInfoEl.pElements = Array.from(weatherInfoEl.querySelectorAll('#weatherInfo p')) || [];
    weatherInfoEl.pElements.forEach(p => p.textContent = '');
    weatherInfoEl.pElements[1].textContent = `Temperature: ${weatherInfo.temperature}`;
    weatherInfoEl.pElements[0].textContent = `City: ${weatherInfo.city}`;
    weatherInfoEl.pElements[2].textContent = `Description: ${weatherInfo.description}`;
}

function updateIpInfo(ipInfoData) {
    ipInfo.pElements = Array.from(ipInfo.querySelectorAll('#ipInfo p')) || [];
    ipInfo.pElements.forEach(p => p.textContent = '');
    ipInfo.pElements[1].textContent = `IP: ${ipInfoData.ip}`;
    ipInfo.pElements[0].textContent = `City: ${ipInfoData.city}`;
}

function updateNewsCarousel(newsArticles) {
    newsList.innerHTML = '';
    const limitedNewsArticles = newsArticles.slice(0, 4);
    limitedNewsArticles.forEach(newsArticle => {
        const newsItem = document.createElement('div');
        newsItem.classList.add('news-item');

        const contentDiv = document.createElement('div');
        contentDiv.classList.add('news-content');

        const titleElement = document.createElement('h3');
        titleElement.textContent = newsArticle.title || 'No Title';
        contentDiv.appendChild(titleElement);

        newsItem.appendChild(contentDiv);

        const newsLink = document.createElement('a');
        newsLink.href = newsArticle.url;
        newsLink.target = '_blank';
        newsLink.classList.add('material-link');
        newsLink.appendChild(newsItem);

        newsList.appendChild(newsLink);
    });
}




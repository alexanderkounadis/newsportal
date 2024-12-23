    
        // Listen for menu item clicks
    const menuItems = document.querySelectorAll('#menu li a');
    menuItems.forEach(item => {
    item.addEventListener('click', async (event) => {
        event.preventDefault(); // Prevent default navigation behavior
        const category = event.target.getAttribute('data-category');
        if (!category) {
            // console.error('Missing data-category attribute.');
            return;
        }
        await fetchArticles(category);
    });
});

// Function to fetch articles from the News API
async function fetchArticles(category) {
    // console.log(`Fetching articles for category: ${category}`);
    
    try {
        const response = await fetch(`/umbraco/api/news/getitems/${category}`, { method: 'GET' });

        // console.log('API Response:', response);

        if (response.ok) {
            const data = await response.json();
            // console.log('Parsed Data:', data);
            displayArticles(data);
    
        } else {
            // console.error(`Error: ${response.status} - ${response.statusText}`);
            alert(`Error fetching articles`);
        }
    } catch (error) {
        // console.error('Error fetching articles:', error);
        alert('Failed to fetch articles. Please try again.');
    }
}

    // Function to display articles in the UI
    function displayArticles(articles) {
        
        const container = document.getElementById("main-content");
        container.innerHTML = ''; // Clear any existing articles

        if (articles.length === 0) {
            container.innerHTML = '<p>No articles found for this category.</p>';
            return;
        }

        container.innerHTML = `
            <div id="news-articles">
            <ul class="articles-list">
                ${articles
                    .map(
                        article => `
                        <li class="article-item">
                        <p><a href="${article.url}" target="_blank">${article.title}</a> 
                        <span class="italic">(${article.source.name}</span>, 
                        ${formatDateToDDMM(article.publishedAt)})</p>
                    </li>
                `
                )
                .join("")}
        </ul>
    </div>
`;
        
        }
        
        function formatDateToDDMM(dateString) {
            const date = new Date(dateString);
            const day = String(date.getDate()).padStart(2, '0');
            const month = String(date.getMonth() + 1).padStart(2, '0'); // Months are 0-indexed
            return `${day}/${month}`;
}
    
    
                        
export default {
    content: [
        "./index.html",
        "./src/**/*.{js,ts,jsx,tsx}",
    ],
    theme: {
        extend: {
            colors: {
                'google-blue': '#4285F4',
                'google-red': '#DB4437',
                'google-yellow': '#F4B400',
                'google-green': '#0F9D58',
                'dark-bg': '#1f2023', // Main background
                'dark-surface': '#2d2e32', // Cards, modals, sidebars
                'dark-surface-hover': '#393b40',
                'dark-border': '#4a4d58',
                'dark-text-primary': '#e8eaed',
                'dark-text-secondary': '#969ba1',
            }
        },
    },
    plugins: [],
}
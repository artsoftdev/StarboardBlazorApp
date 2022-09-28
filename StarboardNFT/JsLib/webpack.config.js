const path = require("path");
// webpack needs to be explicitly required
const webpack = require('webpack');

module.exports = {
    module: {
        rules: [
            {
                test: /\.(js|jsx)$/,
                exclude: /node_modules/,
                use: {
                    loader: "babel-loader"
                }
            }
        ]
    },
    resolve: {
        fallback: {
            "crypto": require.resolve("crypto-browserify"),
            "stream": require.resolve("stream-browserify"),
            "http": require.resolve("stream-http"),
            "https": require.resolve("https-browserify"),
            "assert": require.resolve("assert/"),
            "url": require.resolve("url/"),
            "os": require.resolve("os-browserify/browser")
        },
    },
    output: {
        path: path.resolve(__dirname, '../wwwroot/js'),
        filename: "wallet_lib.js",
        library: "WalletLib"
    },
    plugins: [
        // fix "process is not defined" error:
        // (do "npm install process" before running the build)
        new webpack.ProvidePlugin({
            process: 'process/browser.js',
            Buffer: ['buffer', 'Buffer']
        }),
    ]
};
/// <binding />
var webpack  = require('webpack');
var path = require('path');

module.exports = {
    devtool: 'source-map', //source-map  for production, cheap-module-eval-source-map for dev
    entry: {
        main: "./App/App.jsx"
    },
    output: {
        filename: "wwwroot/dist/main.js",
        publicPath: "/"
    },
    target: 'web',
    module: {
        rules: [
            {
                test: /\.css?$/,
                use: ['style-loader',
                    {
                        loader: 'css-loader',
                        options: {
                            importLoaders: 1,
                            //modules: true,
                            //localIdentName: '[name]__[local]___[hash:base64:5]'
                        }
                    },
                    // {
                    //     loader: 'postcss-loader',
                    //     options: {
                    //         parser: "postcss-js"
                    //     }
                    // }
                ]
            },
            {
                test: /\.(js|jsx)$/, use: 'babel-loader'
            },
            {
                test: /\.(ttf|eot|svg|woff(2)?)(\?[a-z0-9]+)?$/,
                use: ['file-loader']
            },
            {
                test: /\.(png|jpg|jpeg|gif|svg|woff|woff2)$/,
                use: ['url-loader']
            }
        ]
    },
	resolve: {
        extensions: [".js", ".jsx"],
        alias: {         
            "kendo": path.resolve(__dirname, "wwwroot/js/kendo/kendo.all.min.js")
        }
    },
    plugins: [
        require('precss'),
        require('autoprefixer'),
        // new webpack.optimize.UglifyJsPlugin({
          // compress: {
            // warnings: false // https://github.com/webpack/webpack/issues/1496
          // }
        // }),
        new webpack.DefinePlugin({
            'process.env.NODE_ENV': JSON.stringify(process.env.NODE_ENV || 'development')
        }),
        //new webpack.HotModuleReplacementPlugin(),
        // enable HMR globally

        //new webpack.NamedModulesPlugin(),
        // prints more readable module names in the browser console on HMR updates
        // new webpack.ProvidePlugin({
        //     $: "jquery",
        //     jQuery: "jquery"
        // })
    ]
}
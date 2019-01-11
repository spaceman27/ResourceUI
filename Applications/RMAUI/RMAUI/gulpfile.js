/// <binding AfterBuild='default' />
const { resolve } = require('path');
var
    gulp = require('gulp'),
    webpack = require('webpack'),
	browserify = require('browserify'),
    source     = require('vinyl-source-stream');
    dist = "./bundles/",
    concat     = require('gulp-concat'),

    es         = require('event-stream')

    // listEntryOutput = [
    // 	{
	// 		entry: {"main": "./App/App.jsx"}, 
	// 		output: { 
	// 			filename: "wwwroot/dist/main.js",
	// 			publicPath: "/"
	// 		}
	// 	}    
    // ]
;
var WebpackDevServer = require('webpack-dev-server');  
// run webpack with multi entry AND output point
gulp.task("runWebpack", function(cb){
	webpack(require('./webpack.config.js'), function(err, stats) {
		cb();
	});	
});
gulp.task("doBundle", function(cb) {
    var jsFiles = [
        './node_modules/jquery/dist/jquery.min.js',
        './wwwroot/js/kendo/kendo.all.min.js',
		//'./wwwroot/dist/main.js'
    ];
	var cssFiles = [
		'./wwwroot/css/kendo/kendo.common-material.min.css',
		'./wwwroot/css/kendo/kendo.material.min.css',
		'./wwwroot/css/kendo/kendo.material.mobile.min.css'
	];
	
	gulp.src(jsFiles)
	.pipe(concat('bundle.js'))
	.pipe(gulp.dest('./wwwroot/dist/'));
	gulp.src(cssFiles)
	.pipe(concat('bundle.css', {newLine: '\r\n'}))
	.pipe(gulp.dest('./wwwroot/css/kendo/'));
	// listEntryOutput.forEach(function(item){
	// 	var webpackConfig = Object.assign(require('./webpack.config.js'), {
	//     	entry: item.entry,
	//         output: item.output
	//     });	    
	//     webpack(webpackConfig, function(error) {});	
	// });    
	cb();
});
gulp.task('default', gulp.series('runWebpack', 'doBundle'));
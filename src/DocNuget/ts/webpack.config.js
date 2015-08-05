module.exports = {
  entry: './app.ts',
  output: {
    filename: '../wwwroot/bundle.js'
  },
  resolve: {
    extensions: ['', '.ts', '.webpack.js', '.web.js', '.js', '.hbs'],
    alias: {
      'handlebars': 'handlebars/dist/handlebars',
      'fetch': './fetch'
    }
  },
  externals: {
    'bootstrap': 'var $',
    'jquery': 'var $'
  },
  devtool: 'source-map',
  module: {
    loaders: [
      { test: /\.ts$/, loader: 'babel-loader!ts-loader' },
      { test: /\.hbs$/, loader: 'handlebars-loader?helperDirs[]=' + __dirname + '/views/helpers' }
    ]
  }
}

module.exports = {
  entry: './app.ts',
  output: {
    filename: 'bundle.js'
  },
  resolve: {
    extensions: ['', '.ts', '.webpack.js', '.web.js', '.js', '.em'],
    alias: {
      'sammy': 'knottie-sammy',
      'sammy.push_location_proxy': 'knottie-sammy/lib/plugins/sammy.push_location_proxy',
      'sammy.handlebars': 'knottie-sammy/lib/plugins/sammy.handlebars',
      'handlebars': 'handlebars/dist/handlebars'
    }
  },
  externals: {
    'bootstrap': 'var $',
    'jquery': 'var $'
  },
  devtool: 'source-map',
  module: {
    loaders: [
      { test: /\.ts$/, loader: 'ts-loader' }
    ]
  }
}

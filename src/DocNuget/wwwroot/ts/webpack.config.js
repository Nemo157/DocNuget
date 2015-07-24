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
      'handlebars': 'handlebars/dist/handlebars'
    }
  },
  devtool: 'source-map',
  module: {
    loaders: [
      { test: /\.ts$/, loader: 'ts-loader' }
    ]
  }
}

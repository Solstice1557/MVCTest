"use strict";
{
  let path = require("path");

  const webpack = require("webpack");
  const CleanWebpackPlugin = require("clean-webpack-plugin");
  const UglifyJSPlugin = require("uglifyjs-webpack-plugin");
  const ExtractTextPlugin = require("extract-text-webpack-plugin");

  const bundleFolder = "wwwroot/bundle/";

  module.exports = {
    entry: {
      main: "./wwwroot/js/main.js",
      manager: "./wwwroot/js/manager.js"
    },

    devtool: "source-map",

    output: {
      filename: "[name].js",
      path: path.resolve(__dirname, bundleFolder)
    },

    plugins: [
      new CleanWebpackPlugin([bundleFolder]),
      new UglifyJSPlugin({
        compress: { warnings: false },
        output: { comments: false },
        sourceMap: true
      }),
      new ExtractTextPlugin({
        filename: "[name].css",
        allChunks: true
      }),
      new webpack.ProvidePlugin({
         $: "jquery",
         jQuery: "jquery",
         'window.jQuery': "jquery",
         Popper: ["popper.js", "default"]
      })
    ],

    module: {
      loaders: [
        {
          test: /\.js$/,
          exclude: /node_modules/,
          loader: "babel-loader",
          query: {
            plugins: ["transform-runtime"],
            presets: ["es2015"]
          }
        },
        { test: /\.(png|woff|woff2|eot|ttf|svg)$/, loader: "url-loader" }
      ],
      rules: [
        {
          test: /\.js$/,
          exclude: /(node_modules|bower_components)/,
          use: {
            loader: "babel-loader",
            options: {
              presets: ["es2015"]
            }
          }
        },
        {
          test: /\.scss$/,
          use: ExtractTextPlugin.extract(
                {
                  fallback: "style-loader",
                  use: [{
                    loader: "css-loader", 
                    options: {
                      minimize: true,
                      sourceMap: true
                    }
                  }, {
                    loader: "postcss-loader",
                    options: {
                      plugins: function () {
                        return [
                          require("precss"),
                          require("autoprefixer")
                        ];
                      }
                    }
                  },
                  {
                    loader: "sass-loader",
                    options: {
                      sourceMap: true
                    }
                  }]
                })
        }
      ]
    }
  };
}
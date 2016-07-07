var gulp = require("gulp"),
  rimraf = require("rimraf"),
  gutil = require("gulp-util"),
  path = require('path'),
  gexpect = require('gulp-expect-file');


function root(args) {
  args = Array.prototype.slice.call(arguments, 0);
  return path.join.apply(path, [__dirname].concat(args));
}

const paths = {
  webroot: './wwwroot/spa/',
  sourceroot: '../dotnet-core-auth0.Client/dist/'
};

gulp.task('clean', function (done) {
  gutil.log(gutil.colors.yellow('Cleaning wwwroot..'));
  rimraf(paths.webroot + '**/*', done);
});

gulp.task('copy-client', function (done) {
  gutil.log(gutil.colors.yellow('Copying client files; Expecting at least app, vendor, polyfills (js|css). Watch next line. (PASS / FAIL)'));
  return gulp.src(['!' + paths.sourceroot + '**/*.html', paths.sourceroot + '**/*'])
    .pipe(gexpect({ reportMissing: true, reportUnexpected: false },
      [
        // expected files
        '..\\dotnet-core-auth0.Client\\dist\\js\\app.js',
        '..\\dotnet-core-auth0.Client\\dist\\js\\polyfills.js',
        '..\\dotnet-core-auth0.Client\\dist\\js\\vendor.js',
        '..\\dotnet-core-auth0.Client\\dist\\css\\app.css'
      ]))
    .pipe(gulp.dest(paths.webroot));
});

gulp.task('prepare', [ 'clean', 'copy-client' ]);
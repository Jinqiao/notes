var gulp = require('gulp'),
    sass = require('gulp-ruby-sass');

gulp.task('scss', () =>
	  sass('src/*.scss')
	  .pipe(gulp.dest('dest/'))
	 );

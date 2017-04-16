module.exports = function(grunt) {

  // Project configuration.
  grunt.initConfig({
    crx: {
      myPublicExtension: {
      src: "src/**/*",
      dest: "dist/sitecoreanalyticsdevtools.zip",
    },
    }
  });

  // Load the plugin that provides the "uglify" task.  
  grunt.loadNpmTasks('grunt-crx');

  // Default task(s).
  grunt.registerTask('default', ['crx']);

};
require 'rubygems'
require 'spec/rake/spectask'

current_dir = File.expand_path(File.dirname(__FILE__))

Spec::Rake::SpecTask.new do |t|
    t.spec_opts = ['--options', File.join(current_dir, "spec", "spec.opts") ]
end

task :default => :spec

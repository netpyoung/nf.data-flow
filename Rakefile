require 'fileutils'

ROOT = Dir.pwd

INPUT_EXCEL_DIR   = "#{ROOT}/exels"
OUTPUT_DIR        = "#{ROOT}/dataflow_unity/Assets/output"
OUTPUT_DB_PATH    = "#{ROOT}/dataflow_unity/Assets/output/output.db"
PASSWORD          = "helloworld"


# for macOs - net4.6
#ENV['FrameworkPathOverride'] = '/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/4.6-api/'
# DOTNET_RUN32 = 'dotnet mono -f net46 -mo="--arch=32 --debug" --loggerlevel Verbose'
# DOTNET_RUN64 = 'dotnet mono -f net462  -mo="--arch=64 --debug"'

task :default do
  sh 'rake -T'
end

desc 'install dotnet-nf-dataflow'
task :tool_install do
  Dir.chdir('solution') do
    Dir.chdir('dotnet-nf-dataflow') do
      sh 'dotnet pack'
      sh 'dotnet tool install --global --add-source ./nupkg dotnet-nf-dataflow'
    end
  end
end

desc 'uninstall dotnet-nf-dataflow'
task :tool_uninstall do
  sh 'dotnet tool uninstall -g dotnet-nf-dataflow'
end

desc("generate db & dll")
task :dataflow do
  # C:\prj\nf.data-flow\dataflow_unity\Assets\output
  # sh "dotnet nf-dataflow code-gen -i #{INPUT_EXCEL_DIR} -o #{OUTPUT_DIR}"
  sh "dotnet nf-dataflow data-export -i #{INPUT_EXCEL_DIR} -o #{OUTPUT_DB_PATH} -p #{PASSWORD}"
end
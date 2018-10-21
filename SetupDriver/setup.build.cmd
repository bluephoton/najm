@ECHO OFF
REM -----------------------------------------------------------------------------
REM                       Setting some variables
REM -----------------------------------------------------------------------------
SET NAJM_SOL_DIR=%1
SET NAJM_BUILD_FLAVOR=%2
SET NAJM_LAYOUT_DIR=%NAJM_SOL_DIR%SetupDriver\Layout
SET NAJM_OUT_DIR=%NAJM_SOL_DIR%SetupDriver\Out
SET NAJM_VS_BIN_DIR=%NAJM_SOL_DIR%Application\bin\%NAJM_BUILD_FLAVOR%
SET NAJM_WIX_DIR=%NAJM_SOL_DIR%SetupDriver\wix
SET NAJM_SETUPSRC_DIR=%NAJM_SOL_DIR%SetupDriver\Setup\SRC
SET NAJM_SETUPRSRC_DIR=%NAJM_SOL_DIR%SetupDriver\Setup\Resources

SET NAJM_BIN_DIR=%NAJM_LAYOUT_DIR%\Najm\Bin\
SET NAJM_DOC_DIR=%NAJM_LAYOUT_DIR%\Najm\Doc\
SET NAJM_FITS_DIR=%NAJM_LAYOUT_DIR%\Najm\SampleFITS\
SET NAJM_HANDLERS_DIR=%NAJM_LAYOUT_DIR%\Najm\Handlers\

SET NAJM_SETUP_PKGNAME=NajmSetup

ECHO Layout directory is: %NAJM_LAYOUT_DIR%

REM -------------------------------------------------------------------------------------------------------------------
REM                                               Cleaning layout
REM -------------------------------------------------------------------------------------------------------------------
ECHO Najm.Setup: Cleaning layout
DEL /S /Q %NAJM_LAYOUT_DIR%

REM -------------------------------------------------------------------------------------------------------------------
REM                                               Creating layout
REM -------------------------------------------------------------------------------------------------------------------
echo Najm.Setup: creating layout
REM
REM >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> Bin <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
REM
CALL :XCOPYW %NAJM_VS_BIN_DIR%\Najm.exe %NAJM_BIN_DIR%
CALL :XCOPYW %NAJM_VS_BIN_DIR%\Najm.config %NAJM_BIN_DIR%
CALL :XCOPYW %NAJM_VS_BIN_DIR%\Najm.FITSIO.DLL %NAJM_BIN_DIR%
CALL :XCOPYW %NAJM_VS_BIN_DIR%\Najm.Boost.DLL %NAJM_BIN_DIR%
CALL :XCOPYW %NAJM_VS_BIN_DIR%\Najm.LinearAlgebra.DLL %NAJM_BIN_DIR%
CALL :XCOPYW %NAJM_VS_BIN_DIR%\Najm.Handlers.Integration.DLL %NAJM_BIN_DIR%
CALL :XCOPYW %NAJM_VS_BIN_DIR%\Najm.ImagingCore.dll %NAJM_BIN_DIR%
CALL :XCOPYW %NAJM_SOL_DIR%Application\Resources\Najm.ico %NAJM_BIN_DIR%
REM
REM >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> Doc <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
REM
CALL :XCOPYW %NAJM_SOL_DIR%documents\Help.pdf %NAJM_DOC_DIR%
REM
REM >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> Handlers\Default <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
REM
CALL :XCOPYW %NAJM_VS_BIN_DIR%\Najm.Handler.Default.dll %NAJM_HANDLERS_DIR%Default\
REM
REM >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> Handlers\Imaging <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
REM
CALL :XCOPYW %NAJM_VS_BIN_DIR%\Najm.Handler.Imaging.dll %NAJM_HANDLERS_DIR%Imaging\
CALL :XCOPYW %NAJM_VS_BIN_DIR%\Histogram.dll %NAJM_HANDLERS_DIR%Imaging\
CALL :XCOPYW %NAJM_VS_BIN_DIR%\PointsGrid.dll %NAJM_HANDLERS_DIR%Imaging\
REM
REM >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> Handlers\Tables <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
REM
CALL :XCOPYW %NAJM_VS_BIN_DIR%\Najm.Handler.Tables.dll %NAJM_HANDLERS_DIR%Tables\
REM
REM >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> Handlers\IPythonW <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
REM
CALL :XCOPYW %NAJM_VS_BIN_DIR%\Najm.Handler.IPython.dll %NAJM_HANDLERS_DIR%IPythonW\
CALL :XCOPYW %NAJM_VS_BIN_DIR%\IronPython.dll %NAJM_HANDLERS_DIR%IPythonW\
REM
REM >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> Handlers\IPythonSamples <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
REM
SET _PYTHON_SAMPLES_ROOT_=%NAJM_SOL_DIR%Handlers\IPython\IPython\Samples
REM............Template
CALL :XCOPYW %_PYTHON_SAMPLES_ROOT_%\Najm.IPython.Template.py %NAJM_HANDLERS_DIR%IPythonSamples\
REM............Table Fields
CALL :XCOPYW %_PYTHON_SAMPLES_ROOT_%\TableFields\TableFields.py %NAJM_HANDLERS_DIR%IPythonSamples\TableFields\
CALL :XCOPYW %_PYTHON_SAMPLES_ROOT_%\TableFields\tsi.jpg %NAJM_HANDLERS_DIR%IPythonSamples\TableFields\
REM............Console
CALL :XCOPYW %_PYTHON_SAMPLES_ROOT_%\Console\Console.py %NAJM_HANDLERS_DIR%IPythonSamples\Console\
CALL :XCOPYW %_PYTHON_SAMPLES_ROOT_%\Console\consoleicon.png %NAJM_HANDLERS_DIR%IPythonSamples\Console\
REM............Cube2AGIF
CALL :XCOPYW %_PYTHON_SAMPLES_ROOT_%\Cube2GIF\cube2gif.py %NAJM_HANDLERS_DIR%IPythonSamples\Cube2GIF\
CALL :XCOPYW %_PYTHON_SAMPLES_ROOT_%\Cube2GIF\cube2gif.png %NAJM_HANDLERS_DIR%IPythonSamples\Cube2GIF\
CALL :XCOPYW %_PYTHON_SAMPLES_ROOT_%\Cube2GIF\agif.py %NAJM_HANDLERS_DIR%IPythonSamples\Cube2GIF\
CALL :XCOPYW %_PYTHON_SAMPLES_ROOT_%\Cube2GIF\ui.py %NAJM_HANDLERS_DIR%IPythonSamples\Cube2GIF\
REM
REM >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> Sample FITS <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
REM
CALL :XCOPYW %NAJM_SOL_DIR%SetupDriver\Setup\SampleFITS\*.* %NAJM_FITS_DIR%

REM -------------------------------------------------------------------------------------------------------------------
REM                                                invoking wix
REM -------------------------------------------------------------------------------------------------------------------
REM to match root used by warsetup - so that relative paths work for both warsetup and VS
PUSHD %NAJM_SETUPSRC_DIR%
%NAJM_WIX_DIR%\candle.exe %NAJM_SETUPSRC_DIR%\%NAJM_SETUP_PKGNAME%.wxs -ext %NAJM_WIX_DIR%\WixUIExtension.dll -ext %NAJM_WIX_DIR%\WixUtilExtension.dll -out %NAJM_OUT_DIR%\%NAJM_SETUP_PKGNAME%.wixobj
%NAJM_WIX_DIR%\light.exe -ext %NAJM_WIX_DIR%\WixUIExtension.dll -ext %NAJM_WIX_DIR%\WixUtilExtension.dll -cultures:en-US -b "C:\Program Files\Jgaa's Internet\War Setup\Licenses" -b %NAJM_LAYOUT_DIR% -b %NAJM_SETUPRSRC_DIR% -out %NAJM_OUT_DIR%\%NAJM_SETUP_PKGNAME%.msi %NAJM_OUT_DIR%\%NAJM_SETUP_PKGNAME%.wixobj
POPD

goto :END

REM -------------------------------------------------------------------------------------------------------------------
REM                                                 utilities
REM -------------------------------------------------------------------------------------------------------------------
:XCOPYW
echo Copying  [%1]  To  [%2]
XCOPY %1 %2
exit /b

REM -------------------------------------------------------------------------------------------------------------------
:END

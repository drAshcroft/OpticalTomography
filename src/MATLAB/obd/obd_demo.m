function varargout = obd_demo(varargin)
% OBD_DEMO M-file for obd_demo.fig
%
% Demo for Online Blind Deconvolution (OBD) 
% for a sequence of 40 images of Epsilon Lyrae
%
% Copyright (C) 2010 Michael Hirsch

% Begin initialization code - DO NOT EDIT
gui_Singleton = 1;
gui_State = struct('gui_Name',       mfilename, ...
                   'gui_Singleton',  gui_Singleton, ...
                   'gui_OpeningFcn', @obd_demo_OpeningFcn, ...
                   'gui_OutputFcn',  @obd_demo_OutputFcn, ...
                   'gui_LayoutFcn',  [] , ...
                   'gui_Callback',   []);
if nargin && ischar(varargin{1})
    gui_State.gui_Callback = str2func(varargin{1});
end

if nargout
    [varargout{1:nargout}] = gui_mainfcn(gui_State, varargin{:});
else
    gui_mainfcn(gui_State, varargin{:});
end
% End initialization code - DO NOT EDIT


% --- Set the parameters of the demo
function [sf,maxiter,n,clipping,srf] = getparameters()
sf       = [50, 50]; % size of the PSF
maxiter  = [10, 1];  % number of iterations for f and x
n        = 40;       % number of images
clipping = 256;      % maximally acceptable pixel (for saturation correction)
srf      = 1.0;      % superresolution factor


% --- OBD step for a single frame
function processFrame(hObject,handles)
handles.output = hObject;

global idx;

% Initialisation 
if idx == 1, global x; x = []; else, global x; end

% Load image, use only the green color channel
fn = sprintf('./data/epsilon_lyrae/%08d.png', idx);
y  = double(imread(fn));
y  = y(:,:,2); 

% Do OBD step
[sf,maxiter,n,clipping,srf] = getparameters();
[x, f] = obd(x, y, sf, maxiter, clipping, srf);

% Update figures
axes(handles.axes1)
imagesc(y);axis image; axis off; colormap gray
title(sprintf('Observed frame %d/40',idx)); drawnow

cla(handles.axes2,'reset')
axes(handles.axes2)
imagesc(f);axis image; axis off; colormap gray
title('Estimated PSF'); drawnow

cla(handles.axes3,'reset')
axes(handles.axes3)
imagesc(x);axis image; axis off; colormap gray
title('Estimated image'); drawnow

% Description and explanation of what's happening
txt = findobj(gcf,'Tag','description'); 
if idx == 1
    explanation = sprintf([...
    '\n'...
    '\n'...
    'The first observed image (shown in the left panel) is used to initialize \n'...
    'our estimate of the true underlying image (shown in the right panel). \n\n'...
    'Press "Next frame" to load the next image and update the estimated image.']);
    idx = idx + 1;    
elseif idx == 2
    explanation = sprintf([...
  'The next observed image has been loaded (shown in the left panel). This has been\n'...
  'used to estimate the PSF (shown in the middle) that best explains the previous estimate\n'...
  'Using the PSF the estimate of the true underlying image has been estimated as well.\n'...
  '\n'...
  'The remaining images will be loaded automatically.  Watch the figure\n'...
  'to see how the estimate of the true image gets better and better.\n'...
  '\n'...
  'Press "Next frame" to process the next observed frame only\n'...
  'or "Run" for processing all remaining images.']);
    idx = idx + 1;    
elseif idx > 2 && idx < 40
    explanation = sprintf('\n\n\n\n Processing frame %d/40',idx);
    idx = idx + 1;    
elseif idx == 40
    txt = findobj(gcf,'Tag','description'); 
    explanation = sprintf([...
   '\n'...
   '\n'...      
    'All images have been processed. The final estimate for the true\n'...
    'underlying image is shown in the right panel.  We can see clearly\n'...
    'two stars with fewer speckles than the observed images.\n'...
    '\n'...
    'The demonstration is now complete.  Please "Restart" to see the demo again or exit.']);
end
set(txt,'String',explanation);
% Update handles structure
guidata(hObject, handles);


% --- Executes just before obd_demo is made visible.
function obd_demo_OpeningFcn(hObject, eventdata, handles, varargin)
% idx specifies the frame number
global idx;
idx = 1;
processFrame(hObject,handles);


% --- Outputs from this function are returned to the command line.
function varargout = obd_demo_OutputFcn(hObject, eventdata, handles) 
% Get default command line output from handles structure
varargout{1} = handles.output;


% --- Executes on button press in nextButton.
function nextButton_Callback(hObject, eventdata, handles)
processFrame(hObject,handles);


% --- Executes on button press in runButton.
function runButton_Callback(hObject, eventdata, handles)
global idx
for j = [idx:40]
    processFrame(hObject,handles);
end


% --- Executes on button press in restartButton.
function restartButton_Callback(hObject, eventdata, handles)
% Reset index to 1
global idx;
idx = 1;
processFrame(hObject,handles);


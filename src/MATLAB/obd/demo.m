function varargout = demo(varargin)
% DEMO M-file for demo.fig
% 
% Just loads obd_demo.m, Intro to
% 
% Demo for Online Blind Deconvolution (OBD) 
% for a sequence of 40 images of Epsilon Lyrae
%
% Copyright (C) 2010 Michael Hirsch

% Begin initialization code - DO NOT EDIT
gui_Singleton = 1;
gui_State = struct('gui_Name',       mfilename, ...
                   'gui_Singleton',  gui_Singleton, ...
                   'gui_OpeningFcn', @demo_OpeningFcn, ...
                   'gui_OutputFcn',  @demo_OutputFcn, ...
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


% --- Executes just before demo is made visible.
function demo_OpeningFcn(hObject, eventdata, handles, varargin)
handles.output = hObject;
guidata(hObject, handles);


% --- Outputs from this function are returned to the command line.
function varargout = demo_OutputFcn(hObject, eventdata, handles) 
varargout{1} = handles.output;


% --- Outputs from this function are returned to the command line.
function startDemoButton_Callback(hObject, eventdata, handles)
obd_demo


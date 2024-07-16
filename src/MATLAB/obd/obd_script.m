% Script for
% Online Blind Deconvolution (OBD) 
% Version 0.0, 10-Sep-2010
%
%    x     - true underlying image
%    y     - an observed image
%    f     - a point spread function (PSF)
%
% Copyright (C) 2010 by Michael Hirsch & Stefan Harmeling.

clear      % memory

% parameters
sf = [50, 50];       % size of the PSF
maxiter = [50, 1];   % number of iterations for f and x
n = 40;              % number of images
clipping = Inf;      % maximally acceptable pixel (for saturation correction)
srf = 1.0;           % superresolution factor

% how are the filenames generated?
%imagepath = 'data/epsilon_lyrae';
%y_fname = @(i) fullfile(imagepath, sprintf('%08d.png', i));

imagepath = 'C:\temp\registered';
y_fname = @(i) fullfile(imagepath, sprintf('images.tif%03d.tif', i));

% intially there is no x
x = [];

% iterate over all images
for i = 1:n
  % load the next observed image
  fname = y_fname(i);
  fprintf('[%s.m] processing %s\n', mfilename, fname);
  y = imread(fname);
  y = double(y(:,:,1));   % use only first color channel

  %%%%% THE MAIN WORK HORSE %%%%%
  [x, f] = obd(x, y, sf, maxiter, clipping, srf);

  if 1
    % show intermediate output
    clf
    subplot(131), imagesc(y), title(sprintf('observed image y%d', i)); axis equal, axis tight 
    if exist('f', 'var')
      subplot(132), imagesc(f), title(sprintf('estimated PSF f%d', i)); axis equal, axis tight
    end
    subplot(133), imagesc(x), title(sprintf('estimated image x%d', i)); axis equal, axis tight
    colormap gray
    drawnow
  end
end
fprintf('done!  the result is in variable "x", try e.g. "imagesc(x)"\n');

function [newImage,psf] = DeconvolveImageOBD2(ImageArray, origPSF, showWork, srf ,denoise ,denoisemode ,iters,regularize)

f=[];
% parameters
s = size(origPSF);

sf = s;       % size of the PSF
maxiter = iters;   % number of iterations for f and x

clipping = Inf;      % maximally acceptable pixel (for saturation correction)
% intially there is no x
x = [];

clipping =max( max( ImageArray{1} ))*1.2;

% iterate over all images
for i = 1:length(ImageArray)
    
    %f=origPSF(:,:);
    y = double(ImageArray{i});
    
    %%%%% THE MAIN WORK HORSE %%%%%
    [x, f] = obd2(x, y, sf, maxiter, clipping, srf ,f,origPSF,regularize);
    % x( x>clipping)=0;
    if showWork
        startI=round((size(x,1)-size(y,1)*srf)/2);
        x2=x(startI:end-startI,startI:end-startI);
        % show intermediate output
        clf
        subplot(131), imagesc(y), title(sprintf('observed image y%d', i)); axis equal, axis tight
        if exist('f', 'var')
            subplot(132), imagesc(f), title(sprintf('estimated PSF f%d', i)); axis equal, axis tight
        end
        subplot(133), imagesc(x2), title(sprintf('estimated image x%d', i)); axis equal, axis tight
        colormap gray
        drawnow
    end
end

drawnow;



%  startI=round((size(x,1)-size(y,1)*srf)/2);
%  x3=x(startI:end-startI,startI:end-startI);
% graymap(x3);drawnow;
% title('normal');
if denoise
    x=  obd_denoise_xxx(x, f, y,  srf,denoisemode);
else
    startI=round((size(x,1)-size(y,1)*srf)/2);
    x=x(startI:end-startI,startI:end-startI);
    
end
%  graymap(x2);drawnow;
% title('test');
if showWork
    subplot(133), imagesc(x), title(sprintf('estimated image x%d', i)); axis equal, axis tight
    colormap gray
    drawnow
end
psf=f;
newImage=x;

end
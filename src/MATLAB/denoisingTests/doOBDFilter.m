
function [yhat]= doOBDFilter(z1, z2, scale)
    
        yhat = doOBD({z1 flipud(z2)},1, scale); % denoise assuming AWGN
        
      
end

function [yhat]= doOBDFilterO(z1, z2)
    % Poisson scaling factor
    alpha = 1;
    
    % Gaussian component N(g,sigma^2)
    sigma = 1;
    g = 0.0;
    
    fz = GenAnscombe_forward(z1,sigma,alpha,g); % Generalized Anscombe VST (J.L. Starck, F. Murtagh, and A. Bijaoui, Image  Processing  and  Data Analysis, Cambridge University Press, Cambridge, 1998)
        
       
        sigma_den = 1;  % Standard-deviation value assumed after variance-stabiliation
        
        % Scale the image (BM3D processes inputs in [0,1] range)
        scale_range = 1;
        scale_shift = (1-scale_range)/2;
        
        
        maxzans = max(fz(:));
        minzans = min(fz(:));
        fz = (fz-minzans)/(maxzans-minzans);   sigma_den = sigma_den/(maxzans-minzans);
        fz = fz*scale_range+scale_shift;       sigma_den = sigma_den*scale_range;
        
        
        
        
        fz2 = GenAnscombe_forward(z2,sigma,alpha,g); % Generalized Anscombe VST (J.L. Starck, F. Murtagh, and A. Bijaoui, Image  Processing  and  Data Analysis, Cambridge University Press, Cambridge, 1998)
       
        fz2 = (fz2-minzans)/(maxzans-minzans);  
        fz2 = fz2*scale_range+scale_shift;       
        
   
        
        D = doOBD({fz fz2},1); % denoise assuming AWGN
        
        % Scale back to the initial VST range
        D = (D-scale_shift)/scale_range;
        D = D*(maxzans-minzans)+minzans;
        
        % Apply the inverse transformation
        yhat = GenAnscombe_inverse_exact_unbiased(D,sigma,alpha,g);   % exact unbiased inverse
end

function [x] = doOBD(yy,displayGraph,scale)
    % parameters
    sf = [50*scale, 50*scale];       % size of the PSF
    maxiter = [10, 1];   % number of iterations for f and x
    clipping = Inf;      % maximally acceptable pixel (for saturation correction)
    srf =scale;           % superresolution factor

      % intially there is no x
    x = [];
    f=[];
    % iterate over all images
    for i = 1:length(yy)
      % load the next observed image
    
      y = yy{i};  % use only first color channel

      %%%%% THE MAIN WORK HORSE %%%%%
      [x, f] = obd(x, y, sf, maxiter, clipping, srf,f);

      if displayGraph
        % show intermediate output
        clf
        subplot(131), imagesc(y), title(sprintf('observed image y%d', i)); axis equal, axis tight 
        if exist('f', 'var')
          subplot(132), imagesc(f), title(sprintf('estimated PSF f%d', i)); axis equal, axis tight
        end
        subplot(133), imagesc(x), title(sprintf('estimated image x%d', i)); axis equal, axis tight
        %colormap gray
        drawnow
      end
    end
    
    startI=round((size(x,1)-size(y,1)*srf)/2);
    x=x(startI:end-startI,startI:end-startI);
    subplot(133), imagesc(x), title(sprintf('estimated image x%d', i)); axis equal, axis tight
end
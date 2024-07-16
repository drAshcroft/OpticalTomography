
%%%%%%%%%%%%%%%%
function [finalImage] = obd_denoise_xxx(image, psf, y,  srf, mode)
% depending on the value of sf the roles of f and x can be swapped
cleanImage = image(:,:);
finalImage = zeros(size(y));
cleanY=y(:,:);
for C=1:4
    image=cleanImage(:,:);
    y=cleanY(:,:);
    m=zeros(size(y))+1;
    if C==1
        sI=1;
        sJ=1;
    end
    if C==2
        sI=2;
        sJ=1;
    end
    if C==3
        sI=1;
        sJ=2;
    end
    if C==4;
        sI=2;
        sJ=2;
    end
    for I=sI:2:size(m,1)
        for J=sJ:2:size(m,2)
            m(I,J)=0;
        end
    end
    
    sf = size(image);
    sy = size(y);    % for srf > 1, the low resolution y
    
    y = y .* m;                      % deal with clipping
    for i = 1:1
        ytmp = pos(cnv2(psf, image, sy));
        ytmp = ytmp .* m;                 % deal with clipping
        nom = pos(cnv2tp(psf, y, srf));
        denom = pos(cnv2tp(psf, ytmp, srf));
        tol = 1e-10;
        factor = (nom+tol) ./ (denom+tol);
        factor = reshape(factor, sf);
        %   imagesc(factor);drawnow;
        image = image .* factor;
    end
    
    %tImage{C}=image;
     
    startI=round((size(image,1)-size(y,1)*srf)/2);
    x2=image(startI:end-startI,startI:end-startI);
 
    if mode
       m=1-m;
    end
    
    x2=x2.*m;
    
    finalImage =finalImage + x2;
   
end

% 
% 
% finalImage = zeros(size(cleanImage));
% for C=1:4
%     finalImage = finalImage + tImage{C};
% end
% finalImage = finalImage./4;
return;


%%%%%%%%%%%%%%
function x = pos(x, epsilon)
x(find(x(:)<0)) = 0;
return

%%%%%%%%%%%%%%%%%
function A = cnv2slice(A, i, j);
A = A(i,j);
return

%%%%%%%%%%%%%%%%%
function y = cnv2(x, f, sy)
sx = size(x);
sf = size(f);
if all(sx >= sf)   % x is larger or equal to f
  % perform convolution in Fourier space
  y = ifft2(fft2(x) .* fft2(f, sx(1), sx(2)));
  y = cnv2slice(y, sf(1):sx(1), sf(2):sx(2));
elseif all(sx <= sf)  % x is smaller or equal than f
  y = cnv2(f, x, sy);
else
  % x and f are incomparable
  error('[cnv2.m] x must be at least as large as f or vice versa.');
end
if any(sy > size(y))
  error('[cnv2.m] size missmatch');
end
if any(sy < size(y));
  y = samp2(y, sy);   % downsample
end
return

%%%%%%%%%%%%%%%%%
function f = cnv2tp(x, y, srf)
sx = size(x);
if srf > 1
  y = samp2(y, floor(srf*size(y)));    % upsample
end
sy = size(y);
  
% perform the linear convolution in Fourier space
if all(sx >= sy)
  sf = sx - sy + 1;
  tmp=ifft2(conj(fft2(x)).*fft2(cnv2pad(y, sf)));
  f = cnv2slice(tmp, 1:sf(1), 1:sf(2));
elseif all(sx <= sy)
  sf = sy + sx - 1;
  f = ifft2(conj(fft2(x, sf(1), sf(2))).*fft2(cnv2pad(y, sx), sf(1), sf(2)));
else  % x and y are incomparable
  error('[cnv2.m] x must be at least as large as y or vice versa.');
end
f = real(f);  
return

%%%%%%%%%%%%%
function B = cnv2pad(A, sf);
% PAD with zeros from the top-left
i = sf(1);  j = sf(2);
[rA, cA] = size(A);
B = zeros(rA+i-1, cA+j-1);
B(i:end, j:end) = A;
return

%%%%%%%%%%%%%
function y = samp2(x, sy);
sx = size(x);
% downsample by factor srf
y = sampmat(sy(1), sx(1)) * x * sampmat(sy(2), sx(2))';
return

%%%%%%%%%%%%%
function D = sampmat(m, n)
D = kron(speye(m), ones(n, 1)') * kron(speye(n), ones(m, 1))/m;
return
